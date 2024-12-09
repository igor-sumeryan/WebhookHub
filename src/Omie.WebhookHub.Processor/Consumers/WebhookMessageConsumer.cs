using System.Text.Json;
using System.Text.Json.Nodes;
using MassTransit;
using Omie.WebhookHub.Core.Data;
using Omie.WebhookHub.Core.Entities;
using Omie.WebhookHub.Core.Messages;

namespace Omie.WebhookHub.Processor.Consumers;

public class WebhookMessageConsumer : IConsumer<WebhookReceivedMessage>
{
    private readonly WebhookDbContext _dbContext;
    private readonly ILogger<WebhookMessageConsumer> _logger;

    public WebhookMessageConsumer(WebhookDbContext dbContext, ILogger<WebhookMessageConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<WebhookReceivedMessage> context)
    {
        try
        {
            var message = context.Message;
            var jsonNode = JsonSerializer.SerializeToNode(message.Payload) 
                ?? throw new InvalidOperationException("Failed to serialize payload to JsonNode");

            var webhookMessage = new WebhookMessage
            {
                Id = Guid.NewGuid(),
                TimeStamp = message.ReceivedAt,
                Message = jsonNode,
                Type = message.Type,
                Processed = null
            };

            _dbContext.WebhookMessages.Add(webhookMessage);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Webhook stored in database. ID: {Id}, Type: {Type}", 
                webhookMessage.Id, webhookMessage.Type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook message");
            throw;
        }
    }
}