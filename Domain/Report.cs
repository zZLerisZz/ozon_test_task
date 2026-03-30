namespace OzonTestTask.Domain;

/// <summary>
/// Отчет по конверсии
/// </summary>
public class Report {
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public Guid CheckoutId { get; set; }
    public decimal Ratio { get; set; }
    public int PurchasesCount { get; set; }
    public int ViewsCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
