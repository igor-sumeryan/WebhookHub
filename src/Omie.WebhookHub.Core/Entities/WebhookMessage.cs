using System.Text.Json.Nodes;

namespace Omie.WebhookHub.Core.Entities;

public class WebhookMessage
{
    public Guid Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public required JsonNode Message { get; set; }
    public required string Type { get; set; }
    public DateTime? Processed { get; set; }
}