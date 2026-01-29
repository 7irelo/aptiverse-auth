package main

import (
	"log"
	"os"
	"os/signal"
	"syscall"

	"aptiverse-email/internal/config"
	"aptiverse-email/internal/rabbitmq"
)

func main() {
	cfg, err := config.Load()
	if err != nil {
		log.Fatalf("Failed to load config: %v", err)
	}

	consumer, err := rabbitmq.NewConsumer(cfg)
	  
	if err != nil {
		log.Fatalf("Failed to create consumer: %v", err)
	}

	err = consumer.Start()
	if err != nil {
		log.Fatalf("Failed to start consumer: %v", err)
	}

	log.Println("Email service started successfully")

	sigChan := make(chan os.Signal, 1)
	signal.Notify(sigChan, syscall.SIGINT, syscall.SIGTERM)

	<-sigChan
	log.Println("Shutdown signal received")

	consumer.Stop()
	log.Println("Email service stopped gracefully")
}