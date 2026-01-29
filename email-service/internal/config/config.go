package config

import (
	"os"
	"strconv"
)

type Config struct {
	RabbitMQ RabbitMQConfig
	SMTP     SMTPConfig
	App      AppConfig
}

type RabbitMQConfig struct {
	URL       string
	QueueName string
}

type SMTPConfig struct {
	Host     string
	Port     string
	Username string
	Password string
	From     string
}

type AppConfig struct {
	MaxWorkers int
	LogLevel   string
}

func Load() (*Config, error) {
	maxWorkers, _ := strconv.Atoi(getEnv("MAX_WORKERS", "5"))

	return &Config{
		RabbitMQ: RabbitMQConfig{
			URL:       getEnv("RABBITMQ_URL", "amqp://guest:guest@localhost:5672/"),
			QueueName: getEnv("QUEUE_NAME", "email_queue"),
		},
		SMTP: SMTPConfig{
			Host:     getEnv("SMTP_HOST", "smtp.gmail.com"),
			Port:     getEnv("SMTP_PORT", "587"),
			Username: getEnv("SMTP_USER", ""),
			Password: getEnv("SMTP_PASS", ""),
			From:     getEnv("SMTP_FROM", ""),
		},
		App: AppConfig{
			MaxWorkers: maxWorkers,
			LogLevel:   getEnv("LOG_LEVEL", "info"),
		},
	}, nil
}

func getEnv(key, defaultValue string) string {
	if value := os.Getenv(key); value != "" {
		return value
	}
	return defaultValue
}