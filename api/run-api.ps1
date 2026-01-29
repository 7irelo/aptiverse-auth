Write-Host "Starting Aptiverse API container..." -ForegroundColor Green

docker run --name aptiverse-api `
  -p 5196:5196 `
  --network aptiverse-network `
  -e ASPNETCORE_ENVIRONMENT=Development `
  -e ASPNETCORE_URLS=http://0.0.0.0:5196 `
  -e ConnectionStrings__DefaultConnection="Host=aptiverse-db;Database=aptiverse;Username=postgres;Password=password" `
  -e ConnectionStrings__Redis="aptiverse-redis:6379,abortConnect=false" `
  -e Jwt__Key="your-super-secure-jwt-secret-key-at-least-32-characters-long-here" `
  -e Jwt__Issuer="aptiverse-api" `
  -e Jwt__Audience="aptiverse-users" `
  -e Jwt__ExpireHours="4" `
  -e Authentication__Google__ClientId="944718989817-gvvl36isl0dka86jnn7jqnitb961ce3c.apps.googleusercontent.com" `
  -e Authentication__Google__ClientSecret="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" `
  -e EmailSettings__Server="smtp.gmail.com" `
  -e EmailSettings__Port="587" `
  -e EmailSettings__SenderName="Aptiverse" `
  -e EmailSettings__SenderEmail="tirelo.eric@gmail.com" `
  -e EmailSettings__Username="tirelo.eric@gmail.com" `
  -e EmailSettings__Password="zcpadquaqpdppxay" `
  -e RabbitMQSettings__Host="aptiverse-mq" `
  -e RabbitMQSettings__Port="5672" `
  -e RabbitMQSettings__Username="admin" `
  -e RabbitMQSettings__Password="admin" `
  -e RabbitMQSettings__VirtualHost="/" `
  -e RabbitMQSettings__QueueName="email_queue" `
  -d aptiverse-api:dev

if ($LASTEXITCODE -eq 0) {
    Write-Host "Aptiverse API container started successfully!" -ForegroundColor Green
    Write-Host "API available at: http://localhost:5196" -ForegroundColor Cyan
} else {
    Write-Host "Failed to start Aptiverse API container" -ForegroundColor Red
}