using System.Text.Json;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Omie.WebhookHub.Core.Messages;

namespace Omie.WebhookHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(IPublishEndpoint publishEndpoint, ILogger<WebhookController> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveWebhook([FromBody] JsonDocument payload)
    {
        try
        {
            var webhookType = Request.Headers.TryGetValue("X-Webhook-Type", out var type) 
                ? type.ToString() 
                : "Unknown";

            var webhookMessage = new WebhookReceivedMessage
            {
                Payload = payload,
                ReceivedAt = DateTime.UtcNow,
                Type = webhookType
            };

            await _publishEndpoint.Publish(webhookMessage);
            _logger.LogInformation("Webhook received and published to queue. Type: {Type}", webhookType);
            
            return Ok(new { message = "Webhook received successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook");
            return StatusCode(500, new { error = "Error processing webhook" });
        }
    }
}