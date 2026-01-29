FROM golang:1.21-alpine

WORKDIR /app

# Copy go mod files
COPY go.mod go.sum ./
RUN go mod download

# Copy source code
COPY . .

# Build the application
RUN go build -o email-service ./cmd/email-service

# Expose port (if needed for health checks)
EXPOSE 8080

# Run the application
CMD ["./email-service"]