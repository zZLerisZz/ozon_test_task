using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;
/// <summary>
/// Конфигурация для событий(покупка/просмотр)
/// </summary>
public class EventConfiguration: IEntityTypeConfiguration<Event> {
    public void Configure(EntityTypeBuilder<Event> builder) {
        builder.ToTable("events");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(x => x.ProductId).HasColumnName("productid").IsRequired();
        builder.Property(x => x.CheckoutId).HasColumnName("checkoutid").IsRequired();
        builder.Property(x => x.EventTime).HasColumnName("eventtime").HasColumnType("timestamp with time").IsRequired();
        builder.Property(x => x.EventType).HasColumnName("eventtype").HasColumnType("event_type").IsRequired();

        builder.HasIndex(x => new { x.ProductId, x.EventTime, x.EventType, x.CheckoutId }).HasDatabaseName("idx_events_product_time_type");
    }
}
