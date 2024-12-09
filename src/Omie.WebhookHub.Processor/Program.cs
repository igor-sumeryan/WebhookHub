using MassTransit;
using Microsoft.EntityFrameworkCore;
using Omie.WebhookHub.Core.Configuration;
using Omie.WebhookHub.Core.Data;
using Omie.WebhookHub.Processor.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Set environment based on command line argument
var environment = args.Contains("--environment=deploy") ? "Deploy" : "Test";
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure DbContext
builder.Services.AddDbContext<WebhookDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<WebhookMessageConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        cfg.ReceiveEndpoint("webhook-processor", e =>
        {
            e.ConfigureConsumer<WebhookMessageConsumer>(context);
            e.UseMessageRetry(r => r.Intervals(100, 200, 500, 1000, 2000));
        });
    });
});

var app = builder.Build();
app.Run();