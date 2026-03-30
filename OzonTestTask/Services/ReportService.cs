using Domain.Enums;
using Infrastructure.Kafka;
using Microsoft.EntityFrameworkCore;
using OzonTestTask.Domain;
using OzonTestTask.DTOs;
using OzonTestTask.Infrastructure;

namespace OzonTestTask.Services;

public class ReportService: IReportService {
    private readonly AppDbContext _db;
    private readonly IMessage _message;
    
    public ReportService(AppDbContext db, IMessage message) {
        _db = db;
        _message = message;
    }

    public async Task<ReportResponseDTO> CheckRequest(Guid id) {
        var request = await _db.ReportRequests.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        if(request == null)
            throw new ArgumentException("ivalid-request-id");
        return new ReportResponseDTO {
            RequestId = id,
            ReportId = request.ReportId,
            RequestStatus = request.Status
        };
    }

    public async Task<ReportResponseDTO> CreateReportRequest(ReportRequestDTO data) {
        ValidateReportRequest(data);

        var existingReport = await _db.Reports
            .FirstOrDefaultAsync(r =>
                r.ProductId == data.ProductID &&
                r.CheckoutId == data.CheckoutID &&
                r.DateFrom == data.DateFrom &&
                r.DateTo == data.DateTo
            );
        if(existingReport != null) {
            return new ReportResponseDTO {
                RequestId = Guid.Empty,
                ReportId = existingReport.Id,
                RequestStatus = RequestStatus.Done
            };
        }

        var existingRequest = await _db.ReportRequests
            .FirstOrDefaultAsync(r =>
                r.ProductId == data.ProductID &&
                r.CheckoutId == data.CheckoutID &&
                r.DateFrom == data.DateFrom &&
                r.DateTo == data.DateTo
            );
        if(existingRequest != null) {
            return new ReportResponseDTO {
                RequestId = Guid.Empty,
                ReportId = existingRequest.ReportId,
                RequestStatus = existingRequest.Status
            };
        }
        var request = new ReportRequest();
        request = FillReportRequest(request, data);
        _db.ReportRequests.Add(request);
        await _db.SaveChangesAsync();
        await _message.PublishAsync(new ReportRequestEvent {
            RequestId = request.Id,
        });

        return new ReportResponseDTO {
            RequestId = request.Id,
            ReportId = Guid.Empty,
            RequestStatus = RequestStatus.Pending
        };
    }

    public async Task<ReportDTO> GetReport(Guid id) {
        var report = await _db.Reports.FirstOrDefaultAsync(r => r.Id == id);
        if(report == null)
            throw new ArgumentException("invalid-report-id");
        return new ReportDTO {
            Id = report.Id,
            ProductId = report.ProductId,
            DateFrom = report.DateFrom,
            DateTo = report.DateTo,
            CheckoutId = report.CheckoutId,
            ViewsCount = report.ViewsCount,
            Ratio = report.Ratio,
            PurchasesCount = report.PurchasesCount,
            CreatedAt = report.CreatedAt
        };
    }

    private ReportRequest FillReportRequest(ReportRequest request, ReportRequestDTO data) {
        request.Id = Guid.NewGuid();
        request.ProductId = data.ProductID;
        request.DateFrom = data.DateFrom;
        request.DateTo = data.DateTo;
        request.CheckoutId = data.CheckoutID;
        request.Status = RequestStatus.Pending;
        request.CreatedAt = DateTime.UtcNow;
        return request;
    }

    private void ValidateReportRequest(ReportRequestDTO data) {
        if(data.ProductID == Guid.Empty)
            throw new ArgumentException("productid-is-null");
        if(data.CheckoutID == Guid.Empty)
            throw new ArgumentException("checkoutid-is-null");
        if(data.DateFrom > data.DateTo)
            throw new ArgumentException("incorrect-time-interval");
    }
}

