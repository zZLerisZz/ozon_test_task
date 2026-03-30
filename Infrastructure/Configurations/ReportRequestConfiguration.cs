using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OzonTestTask.Domain;

namespace OzonTestTask.Infrastructure.Configurations;

/// <summary>
/// Конфигурация для запросов на отчет
/// </summary>
public class ReportRequestConfiguration: IEntityTypeConfiguration<ReportRequest> {
    public void Configure(EntityTypeBuilder<ReportRequest> builder) {
        builder.ToTable("reportrequests");

        builder.HasKey(x => x.Id);

        builder.Property(x =>  x.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(x => x.ProductId).HasColumnName("productid").IsRequired();
        builder.Property(x => x.DateFrom).HasColumnName("datefrom").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DateTo).HasColumnName("dateto").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("createdat").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.CheckoutId).HasColumnName("checkoutid").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("report_request_status").IsRequired();
        builder.Property(x => x.ReportId).HasColumnName("reportid").IsRequired(false);

        builder.HasIndex(x => new { x.ProductId, x.DateFrom, x.DateTo, x.CheckoutId })
            .IsUnique()
            .HasDatabaseName("ux_reportrequests_unique");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("idx_reportrequests_status");
    }
}
