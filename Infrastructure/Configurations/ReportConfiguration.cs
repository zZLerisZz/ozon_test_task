using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OzonTestTask.Domain;

namespace OzonTestTask.Infrastructure.Configurations;
    
/// <summary>
/// Конфигурация для для отчета по конверсии
/// </summary>
public class ReportConfiguration: IEntityTypeConfiguration<Report> {
    public void Configure(EntityTypeBuilder<Report> builder) {
        builder.ToTable("reports");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(x => x.ProductId).HasColumnName("productid").IsRequired();
        builder.Property(x => x.DateFrom).HasColumnName("datefrom").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DateTo).HasColumnName("dateto").HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.CheckoutId).HasColumnName("checkoutid").IsRequired();
        builder.Property(x => x.Ratio).HasColumnName("ratio").HasColumnType("decimal(10,4)").IsRequired();
        builder.Property(x => x.PurchasesCount).HasColumnName("purchasescount").IsRequired();
        builder.Property(x => x.ViewsCount).HasColumnName("viewscount").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("createdat").HasColumnType("timestamp with time zone").IsRequired();

        builder.HasIndex(x => new { x.ProductId, x.DateFrom, x.DateTo, x.CheckoutId }).IsUnique().HasDatabaseName("ux_reports_unique");
        builder.HasIndex(x => new { x.ProductId, x.DateFrom, x.DateTo, x.CheckoutId }).HasDatabaseName("idx_reports_lookup");
    }
}
