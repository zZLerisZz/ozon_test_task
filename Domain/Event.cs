using Domain.Enums;

namespace Domain;

/// <summary>
/// Событие - покупка/просмотр товара
/// </summary>
public class Event {
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public DateTime EventTime { get; set; }
    public EventType EventType { get; set; }
    public Guid CheckoutId { get; set; }
}
