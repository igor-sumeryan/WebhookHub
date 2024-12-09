namespace Omie.WebhookHub.Core.Models;

public class WebhookPayload
{
    public Guid Id { get; set; }
    public required string RawPayload { get; set; }
    public DateTime ReceivedAt { get; set; }
    public required Dictionary<string, string> Headers { get; set; }
    public DateTime ProcessedAt { get; set; }
}