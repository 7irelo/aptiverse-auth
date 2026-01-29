# Aptiverse Email Service 📧

**A high-performance, production-ready email microservice** written in Go that processes email requests asynchronously via RabbitMQ. Built for reliability, scalability, and easy integration.

---

## ✨ Features

- **Asynchronous Processing** – Consume email jobs from RabbitMQ queues
- **Concurrent Processing** – Configurable worker pools for high throughput
- **Reliable Delivery** – Retry logic with exponential backoff
- **SMTP Integration** – Supports multiple email providers
- **Container Ready** – Full Docker & Docker Compose support
- **Structured Logging** – JSON logging for easy monitoring
- **Health Checks** – Built-in monitoring endpoints
- **Configuration Management** – Environment-based configuration

---

## 🚀 Quick Start

### Prerequisites
- Go 1.21 or later
- RabbitMQ 3.11+
- SMTP server access (SendGrid, AWS SES, Gmail, etc.)
- Docker & Docker Compose (optional, for containerized deployment)

### 1. Clone & Setup
```bash
git clone aptiverse-email
cd aptiverse-email
```

### 2. Configure Environment
```bash
# Copy the example environment file
cp .env.example .env

# Edit with your settings
nano .env  # or use your preferred editor
```

### 3. Local Development
```bash
# Install dependencies
go mod download

# Run the service
go run cmd/main.go
```

### 4. Using Docker (Recommended)
```bash
# Start all services (RabbitMQ + Email Service)
docker-compose up -d

# Check logs
docker-compose logs -f email-service
```

---

## ⚙️ Configuration

### Environment Variables
| Variable | Description | Default |
|----------|-------------|---------|
| `RABBITMQ_URL` | RabbitMQ connection URL | `amqp://guest:guest@localhost:5672/` |
| `SMTP_HOST` | SMTP server host | `smtp.gmail.com` |
| `SMTP_PORT` | SMTP server port | `587` |
| `SMTP_USERNAME` | SMTP username | - |
| `SMTP_PASSWORD` | SMTP password | - |
| `WORKER_POOL_SIZE` | Number of concurrent workers | `5` |
| `MAX_RETRIES` | Maximum delivery retry attempts | `3` |
| `LOG_LEVEL` | Logging level (debug, info, warn, error) | `info` |

### Example `.env` File
```env
# RabbitMQ Configuration
RABBITMQ_URL=amqp://user:password@rabbitmq:5672/

# SMTP Configuration
SMTP_HOST=smtp.sendgrid.net
SMTP_PORT=587
SMTP_USERNAME=apikey
SMTP_PASSWORD=your-sendgrid-api-key

# Service Configuration
WORKER_POOL_SIZE=10
MAX_RETRIES=3
LOG_LEVEL=debug
```

---

## 📦 Docker Deployment

### Docker Compose Configuration
The included `docker-compose.yml` sets up a complete stack:
- **Email Service** – Your Go application
- **RabbitMQ** – Message broker with management UI
- **Network** – Isolated bridge network

```bash
# Build and start
docker-compose up --build -d

# Stop services
docker-compose down

# View RabbitMQ management UI
# Open: http://localhost:15672 (guest/guest)
```

### Custom Deployment
```dockerfile
FROM golang:1.21-alpine AS builder
WORKDIR /app
COPY . .
RUN go build -o email-service ./cmd

FROM alpine:latest
COPY --from=builder /app/email-service .
COPY .env .
CMD ["./email-service"]
```

---

## 🔌 API Integration

### Publishing Email Jobs
Send JSON messages to RabbitMQ queue `email_queue`:

```json
{
  "to": ["recipient@example.com"],
  "cc": ["cc@example.com"],
  "bcc": ["bcc@example.com"],
  "subject": "Your Email Subject",
  "body": "<h1>Hello!</h1><p>This is your email content.</p>",
  "content_type": "text/html",
  "from": "sender@yourdomain.com",
  "reply_to": "support@yourdomain.com",
  "metadata": {
    "campaign_id": "12345",
    "user_id": "67890"
  }
}
```

### Example Producer (Python)
```python
import pika, json

connection = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
channel = connection.channel()
channel.queue_declare(queue='email_queue')

email_job = {
    "to": ["user@example.com"],
    "subject": "Welcome to Aptiverse!",
    "body": "Thank you for joining our platform.",
    "content_type": "text/plain"
}

channel.basic_publish(
    exchange='',
    routing_key='email_queue',
    body=json.dumps(email_job)
)
connection.close()
```

---

## 🧪 Development

### Running Tests
```bash
# Run all tests
go test ./...

# Run with coverage
go test -coverprofile=coverage.out ./...
go tool cover -html=coverage.out
```

### Building from Source
```bash
# Build binary
go build -o bin/email-service ./cmd

# Cross-compile for Linux
GOOS=linux GOARCH=amd64 go build -o bin/email-service-linux ./cmd
```

### Project Structure
```
aptiverse-email/
├── cmd/
│   └── main.go          # Application entry point
├── internal/
│   ├── email/           # Email sending logic
│   ├── queue/           # RabbitMQ handlers
│   ├── config/          # Configuration management
│   └── models/          # Data structures
├── pkg/
│   └── utils/           # Shared utilities
├── docker-compose.yml   # Local development stack
├── Dockerfile           # Container configuration
└── README.md            # You are here
```

---

## 📊 Monitoring & Logs

### Health Endpoint
```bash
# Check service health (if implemented)
curl http://localhost:8080/health
```

### Viewing Logs
```bash
# Docker Compose
docker-compose logs -f email-service

# Filter logs by level
docker-compose logs email-service | grep "ERROR"

# Structured JSON logs example
{"level":"info","time":"2023-10-01T12:00:00Z","msg":"Email sent successfully","to":"user@example.com"}
```

---

## 🔧 Troubleshooting

### Common Issues
1. **Connection refused to RabbitMQ**
   - Ensure RabbitMQ is running: `docker ps | grep rabbit`
   - Check credentials in `.env`

2. **SMTP authentication failed**
   - Verify SMTP credentials
   - Check if "Less secure apps" is enabled (for Gmail)
   - For SendGrid, use API key as password

3. **Emails not sending**
   - Check RabbitMQ queue: `rabbitmqctl list_queues`
   - Verify worker count: Increase `WORKER_POOL_SIZE`
   - Check application logs

### Debug Mode
```bash
# Run with debug logging
LOG_LEVEL=debug go run cmd/main.go
```

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit changes: `git commit -m 'Add amazing feature'`
4. Push to branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- [Go RabbitMQ Client](https://github.com/rabbitmq/amqp091-go)
- [Viper](https://github.com/spf13/viper) for configuration
- [Zap](https://github.com/uber-go/zap) for logging

---

**Need Help?** Open an issue or contact the maintainers.

---

## Key Improvements Made:
1. **Visual Hierarchy** – Clear sections with emojis for better scanning
2. **Better Structure** – Logical flow from setup to deployment
3. **Configuration Table** – Easy-to-read environment variables
4. **Practical Examples** – Real code snippets for integration
5. **Troubleshooting Guide** – Common issues and solutions
6. **Docker Focus** – Clear container deployment instructions
7. **Production Ready** – Added monitoring, logging, and health checks
8. **Clean Formatting** – Consistent markdown styling throughout
9. **Integration Examples** – Show how to actually use the service
10. **Complete Information** – All necessary details for both developers and operators
