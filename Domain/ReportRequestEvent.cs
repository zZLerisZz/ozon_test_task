namespace OzonTestTask.Domain;

/// <summary>
/// Контракт для работы Kafka
/// </summary>    
public class ReportRequestEvent {
    public Guid RequestId { get; set; }
}
