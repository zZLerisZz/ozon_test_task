using Domain.Enums;

namespace OzonTestTask.DTOs;
public class ReportResponseDTO {
    public Guid RequestId { get; set; }    
    public Guid? ReportId { get; set; }
    public RequestStatus RequestStatus { get; set; }
}
