using Aptiverse.Core.Services;
using Aptiverse.Infrastructure.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;

namespace Aptiverse.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly ILogger<EmailSender> _logger;
        private readonly ConnectionFactory _rabbitMqFactory;

        public EmailSender(
            IOptions<EmailSettings> emailSettings,
            IOptions<RabbitMQSettings> rabbitMQSettings,
            ILogger<EmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _rabbitMQSettings = rabbitMQSettings.Value;
            _logger = logger;

            _rabbitMqFactory = new ConnectionFactory()
            {
                HostName = _rabbitMQSettings.Host ?? string.Empty,
                Port = _rabbitMQSettings.Port,
                UserName = _rabbitMQSettings.Username ?? string.Empty,
                Password = _rabbitMQSettings.Password ?? string.Empty,
                VirtualHost = _rabbitMQSettings.VirtualHost ?? string.Empty
            };
        }

        public async Task SendTemplateEmailAsync(string email, string subject, string templateType, object templateData)
        {
            try
            {
                var emailMessage = new
                {
                    To = email,
                    Subject = subject,
                    Timestamp = DateTime.UtcNow,
                    From = _emailSettings.SenderEmail,
                    SenderName = _emailSettings.SenderName,
                    TemplateType = templateType,
                    FirstName = GetPropertyValue(templateData, "FirstName"),
                    LastName = GetPropertyValue(templateData, "LastName"),
                    UserName = GetPropertyValue(templateData, "UserName"),
                    Email = GetPropertyValue(templateData, "Email"),
                    UserType = GetPropertyValue(templateData, "UserType"),
                    ConfirmationLink = GetPropertyValue(templateData, "ConfirmationLink")
                };

                var messageBody = JsonSerializer.Serialize(emailMessage);

                await PublishToRabbitMQ(messageBody);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Template email queued for {Email} via RabbitMQ", email);
                }
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(ex, "Failed to queue template email for {Email}", email);
                }
                throw;
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var emailMessage = new
                {
                    To = email,
                    Subject = subject,
                    Body = htmlMessage,
                    Timestamp = DateTime.UtcNow,
                    From = _emailSettings.SenderEmail,
                    SenderName = _emailSettings.SenderName,
                    TemplateType = "raw_html"
                };

                var messageBody = JsonSerializer.Serialize(emailMessage);

                await PublishToRabbitMQ(messageBody);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Email queued for {Email} via RabbitMQ", email);
                }
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(ex, "Failed to queue email for {Email}", email);
                }
                throw;
            }
        }

        private async Task PublishToRabbitMQ(string messageBody)
        {
            using var connection = await _rabbitMqFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: _rabbitMQSettings.QueueName ?? "email_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = new ReadOnlyMemory<byte>(System.Text.Encoding.UTF8.GetBytes(messageBody));

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _rabbitMQSettings.QueueName ?? "email_queue",
                body: body);
        }

        private static string? GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null) return null;

            var property = obj.GetType().GetProperty(propertyName);
            return property?.GetValue(obj)?.ToString();
        }
    }
}