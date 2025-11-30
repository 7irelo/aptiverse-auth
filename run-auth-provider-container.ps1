# Build and Run Aptiverse Auth Provider Docker Container

Write-Host "🚀 Building and starting Aptiverse Auth Provider..." -ForegroundColor Cyan

# Build the Docker image
Write-Host "📦 Building Docker image..." -ForegroundColor Yellow
docker build --no-cache -t aptiverse-auth-provider .

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker build failed!" -ForegroundColor Red
    exit 1
}

# Stop and remove existing container if it exists
Write-Host "🛑 Stopping and removing existing container..." -ForegroundColor Yellow
docker stop aptiverse-auth-provider 2>$null
docker rm aptiverse-auth-provider 2>$null

# Create network if it doesn't exist
Write-Host "🌐 Setting up Docker network..." -ForegroundColor Yellow
docker network create aptiverse-network 2>$null

# Run the container
Write-Host "🐳 Starting new container..." -ForegroundColor Yellow
docker run -d `
    --name aptiverse-auth-provider `
    --network aptiverse-network `
    -p 5006:5006 `
    -e ASPNETCORE_ENVIRONMENT="Development" `
    -e ASPNETCORE_URLS="http://0.0.0.0:5006" `
    -e ConnectionStrings__DefaultConnection="Host=aptiverse-db;Database=aptiverse;Username=postgres;Password=password" `
    -e ConnectionStrings__Redis="aptiverse-redis:6379,abortConnect=false" `
    -e Jwt__Key="your-super-secure-jwt-secret-key-at-least-32-characters-long-here" `
    -e Jwt__Issuer="aptiverse-api" `
    -e Jwt__Audience="aptiverse-users" `
    -e Jwt__ExpireHours=4 `
    -e Authentication__Google__ClientId="944718989817-gvvl36isl0dka86jnn7jqnitb961ce3c.apps.googleusercontent.com" `
    -e Authentication__Google__ClientSecret="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" `
    -e EmailSettings__Server="smtp.gmail.com" `
    -e EmailSettings__Port=587 `
    -e EmailSettings__SenderName="Aptiverse" `
    -e EmailSettings__SenderEmail="tirelo.eric@gmail.com" `
    -e EmailSettings__Username="tirelo.eric@gmail.com" `
    -e EmailSettings__Password="zcpadquaqpdppxay" `
    -e EmailSettings__RabbitMQHost="rabbitmq" `
    -e EmailSettings__RabbitMQPort="5672" `
    -e EmailSettings__RabbitMQUsername="guest" `
    -e EmailSettings__RabbitMQPassword="guest" `
    -e EmailSettings__RabbitMQVirtualHost="/" `
    -e EmailSettings__QueueName="email_queue" `
    aptiverse-auth-provider

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to start container!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Container started successfully!" -ForegroundColor Green
Write-Host "🌐 Access at: http://localhost:5006" -ForegroundColor Green
Write-Host "📋 Waiting for container to initialize..." -ForegroundColor Yellow

Start-Sleep 10

# Show recent logs
Write-Host "📜 Container logs (last 20 lines):" -ForegroundColor Cyan
docker logs aptiverse-auth-provider --tail 20

# Open Chrome
Write-Host "🌐 Opening Chrome..." -ForegroundColor Yellow
Start-Process "chrome.exe" "http://localhost:5006"

Write-Host "`n🎉 Deployment complete!" -ForegroundColor Green
Write-Host "🔗 Application URL: http://localhost:5006" -ForegroundColor White -BackgroundColor DarkBlue
Write-Host "📊 View logs: docker logs -f aptiverse-auth-provider" -ForegroundColor Gray
Write-Host "🛑 Stop container: docker stop aptiverse-auth-provider" -ForegroundColor Gray