using System.Text.Json;

namespace Omie.WebhookHub.Core.Messages;

public class WebhookReceivedMessage
{
    public required JsonDocument Payload { get; set; }
    public DateTime ReceivedAt { get; set; }
    public required string Type { get; set; }
}