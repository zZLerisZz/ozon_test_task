using Domain.Enums;

namespace OzonTestTask.Domain;

/// <summary>
/// Запрос на отчет по конверсии
/// </summary>
public class ReportRequest {
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public Guid CheckoutId { get; set; }
    public RequestStatus Status { get; set; }
    public Guid? ReportId { get; set; }
    public DateTime CreatedAt { get; set; }
}
