using Microsoft.EntityFrameworkCore;
using Omie.WebhookHub.Core.Entities;

namespace Omie.WebhookHub.Core.Data;

public class WebhookDbContext : DbContext
{
    public WebhookDbContext(DbContextOptions<WebhookDbContext> options) : base(options)
    {
    }

    public DbSet<WebhookMessage> WebhookMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WebhookMessage>(entity =>
        {
            entity.ToTable("omie_webhooks", "public");

            entity.HasKey(e => e.Id)
                .HasName("omie_webhooks_pkey");

            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever();

            entity.Property(e => e.TimeStamp)
                .HasColumnName("TimeStamp")
                .HasDefaultValueSql("now()")
                .IsRequired();

            entity.Property(e => e.Message)
                .HasColumnName("Message")
                .HasColumnType("json")
                .IsRequired();

            entity.Property(e => e.Type)
                .HasColumnName("Type")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.Processed)
                .HasColumnName("Processed")
                .HasColumnType("timestamptz");
        });

        base.OnModelCreating(modelBuilder);
    }
}