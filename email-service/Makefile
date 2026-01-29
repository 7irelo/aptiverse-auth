.PHONY: run build test docker-build docker-run clean help

# Build and run the application
run:
	go run ./cmd/email-service

# Build the binary
build:
	go build -o bin/email-service ./cmd/email-service

# Run tests
test:
	go test ./...

# Run tests with coverage
test-coverage:
	go test -coverprofile=coverage.out ./...
	go tool cover -html=coverage.out -o coverage.html

# Build Docker image
docker-build:
	docker build -t aptiverse-email-service -f deployments/Dockerfile .

# Run with Docker Compose
docker-run:
	docker-compose -f deployments/docker-compose.yml up

# Run in development mode with hot reload (if you have air installed)
dev:
	air

# Clean build artifacts
clean:
	rm -rf bin/
	rm -f coverage.out coverage.html

# Format code
fmt:
	go fmt ./...

# Vet code
vet:
	go vet ./...

# Tidy dependencies
tidy:
	go mod tidy

# Show help
help:
	@echo "Available targets:"
	@echo "  run           - Build and run the application"
	@echo "  build         - Build the binary"
	@echo "  test          - Run tests"
	@echo "  test-coverage - Run tests with coverage report"
	@echo "  docker-build  - Build Docker image"
	@echo "  docker-run    - Run with Docker Compose"
	@echo "  dev           - Run in development mode (requires air)"
	@echo "  clean         - Clean build artifacts"
	@echo "  fmt           - Format code"
	@echo "  vet           - Vet code"
	@echo "  tidy          - Tidy dependencies"
	@echo "  help          - Show this help message"