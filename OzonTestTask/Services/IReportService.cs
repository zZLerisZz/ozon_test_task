using OzonTestTask.DTOs;

namespace OzonTestTask.Services;
    
public interface IReportService {
    Task<ReportResponseDTO> CheckRequest(Guid id);
    Task<ReportResponseDTO> CreateReportRequest(ReportRequestDTO data);
    Task<ReportDTO> GetReport(Guid id);
}
