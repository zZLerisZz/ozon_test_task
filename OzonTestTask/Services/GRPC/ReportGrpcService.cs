using Grpc.Core;
using GrpcReportService;
using Microsoft.EntityFrameworkCore;
using OzonTestTask.Infrastructure;

namespace OzonTestTask.Services.GRPC;
    
public class ReportGrpcService: GrpcReportService.ReportService.ReportServiceBase {
    private readonly AppDbContext _db;

    public ReportGrpcService(AppDbContext db) {
        _db = db;
    }

    public override async Task<GetReportByIdResponse> GetReportById(GetReportByIdRequest request, ServerCallContext context) {
        if(!Guid.TryParse(request.ReportId, out var reportId)) {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "invalid-reportid"));
        }
        var report = await _db.Reports.FirstOrDefaultAsync(r => r.Id == reportId);
        if(report == null) {
            throw new RpcException(new Status(StatusCode.NotFound, "report-not-found"));
        }
        return new GetReportByIdResponse {
            ReportId = report.Id.ToString(),
            ProductId = report.ProductId.ToString(),
            CheckoutId = report.CheckoutId.ToString(),
            DateFrom = report.DateFrom.ToString("O"),
            DateTo = report.DateTo.ToString("O"),
            ViewsCount = report.ViewsCount,
            PurchasesCount = report.PurchasesCount,
            Ratio = (double)report.Ratio
        };
    }
}
