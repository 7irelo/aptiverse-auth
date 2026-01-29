package rabbitmq

import (
	"encoding/json"
	"log"
	"sync"

	"aptiverse-email/internal/config"
	"aptiverse-email/internal/email"
	"aptiverse-email/internal/handlers"
	"aptiverse-email/internal/models"

	amqp "github.com/rabbitmq/amqp091-go"
)

type Consumer struct {
	config    *config.Config
	conn      *amqp.Connection
	channel   *amqp.Channel
	emailSvc  *email.Sender
	isRunning bool
	wg        sync.WaitGroup
}

func NewConsumer(cfg *config.Config) (*Consumer, error) {
	conn, err := amqp.Dial(cfg.RabbitMQ.URL)
	if err != nil {
		return nil, err
	}

	channel, err := conn.Channel()
	if err != nil {
		return nil, err
	}

	_, err = channel.QueueDeclare(
		cfg.RabbitMQ.QueueName,
		true,
		false,
		false,
		false,
		nil,
	)
	if err != nil {
		return nil, err
	}

	err = channel.Qos(
		cfg.App.MaxWorkers,
		0,
		false,
	)
	if err != nil {
		return nil, err
	}

	emailSvc := email.NewSender(cfg)

	return &Consumer{
		config:    cfg,
		conn:      conn,
		channel:   channel,
		emailSvc:  emailSvc,
		isRunning: true,
	}, nil
}

func (c *Consumer) Start() error {
    log.Printf("Declaring queue: %s", c.config.RabbitMQ.QueueName)
    
    msgs, err := c.channel.Consume(
        c.config.RabbitMQ.QueueName,
        "",
        false,
        false,
        false,
        false,
        nil,
    )
    if err != nil {
        return err
    }

    log.Printf("Successfully started consuming from queue: %s", c.config.RabbitMQ.QueueName)

    for i := 0; i < c.config.App.MaxWorkers; i++ {
        c.wg.Add(1)
        go c.worker(msgs, i)
    }

    log.Printf("Started %d email workers", c.config.App.MaxWorkers)
    return nil
}

func (c *Consumer) worker(msgs <-chan amqp.Delivery, workerID int) {
	defer c.wg.Done()

	for c.isRunning {
		msg, ok := <-msgs
		if !ok {
			return
		}

		var emailReq models.EmailRequest
		if err := json.Unmarshal(msg.Body, &emailReq); err != nil {
			log.Printf("Worker %d: Failed to parse message: %v", workerID, err)
			msg.Nack(false, false)
			continue
		}

		if err := handlers.HandleEmailMessage(&emailReq, c.emailSvc); err != nil {
			log.Printf("Worker %d: Failed to process email: %v", workerID, err)
			msg.Nack(false, true)
		} else {
			msg.Ack(false)
			log.Printf("Worker %d: Successfully sent email to %s", workerID, emailReq.To)
		}
	}
}

func (c *Consumer) Stop() {
	c.isRunning = false
	c.wg.Wait()
	c.channel.Close()
	c.conn.Close()
	log.Println("RabbitMQ consumer stopped")
}