using MassTransit;
using Microsoft.EntityFrameworkCore;
using Omie.WebhookHub.Core.Configuration;
using Omie.WebhookHub.Core.Data;
using Omie.WebhookHub.Core.Messages;

var builder = WebApplication.CreateBuilder(args);

// Set environment based on command line argument
var environment = args.Contains("--environment=deploy") ? "Deploy" : "Test";
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext
builder.Services.AddDbContext<WebhookDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();