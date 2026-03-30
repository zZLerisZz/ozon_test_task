namespace OzonTestTask.DTOs;
    
public class ReportRequestDTO {
    public Guid ProductID { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public Guid CheckoutID { get; set; }
}
