# WebhookHub

A webhook receiver and processor system built with .NET 8, RabbitMQ, and PostgreSQL.

## Project Structure

The solution consists of three main projects:

1. **Omie.WebhookHub.Api**: REST API endpoint for receiving webhooks
   - Receives webhook payloads
   - Publishes messages to RabbitMQ

2. **Omie.WebhookHub.Core**: Shared library containing common code
   - Domain models
   - Database contexts
   - Shared configurations

3. **Omie.WebhookHub.Processor**: Background service for processing webhooks
   - Subscribes to RabbitMQ queue
   - Processes webhook messages
   - Stores data in PostgreSQL

## Prerequisites

- .NET 8 SDK
- PostgreSQL
- RabbitMQ

## Configuration

The application supports two environments:

### Test Environment
```json
{
  "ConnectionStrings": {
    "RabbitMQ": "amqp://guest:guest@localhost:5672",
    "PostgreSQL": "Host=localhost;Port=5432;Database=bpo;Username=postgres;Password=@qowtaw%7hyzGacyvtug#"
  }
}
```

### Deploy Environment
```json
{
  "ConnectionStrings": {
    "RabbitMQ": "amqp://guest:guest@rabbitmq-omie.rabbitmq-system.svc.cluster.local:5672",
    "PostgreSQL": "Host=cnpg-cluster-rw.cnpg-system.svc.cluster.local;Port=5432;Database=bpo;Username=postgres;Password=@qowtaw%7hyzGacyvtug#"
  }
}
```

## Running the Application

To run in test environment:
```bash
dotnet run --environment=test
```

To run in deploy environment:
```bash
dotnet run --environment=deploy
```

## PostgreSQL Schema

```sql
CREATE TABLE public.omie_webhooks (
    "Id" uuid NOT NULL,
    "TimeStamp" timestamp DEFAULT now() NOT NULL,
    "Message" json NOT NULL,
    "Type" varchar(30) NOT NULL,
    "Processed" timestamptz NULL,
    CONSTRAINT omie_webhooks_pkey PRIMARY KEY ("Id")
);
```