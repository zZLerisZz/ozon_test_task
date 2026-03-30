using Domain;
using Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OzonTestTask.Domain;
using OzonTestTask.Infrastructure;

public class WorkerTests {
    private AppDbContext CreateDb() {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task ShouldCreateReportAndSetDoneStatus() {
        var db = CreateDb();
        var productId = Guid.NewGuid();
        var checkoutId = Guid.NewGuid();
        db.Events.AddRange(
            new Event {
                Id = Guid.NewGuid(),
                ProductId = productId,
                CheckoutId = checkoutId,
                EventType = EventType.View,
                EventTime = DateTime.UtcNow
            },
            new Event {
                Id = Guid.NewGuid(),
                ProductId = productId,
                CheckoutId = checkoutId,
                EventType = EventType.View,
                EventTime = DateTime.UtcNow
            },
            new Event {
                Id = Guid.NewGuid(),
                ProductId = productId,
                CheckoutId = checkoutId,
                EventType = EventType.Purchase,
                EventTime = DateTime.UtcNow
            }
        );
        var request = new ReportRequest {
            Id = Guid.NewGuid(),
            ProductId = productId,
            CheckoutId = checkoutId,
            DateFrom = DateTime.UtcNow.AddDays(-1),
            DateTo = DateTime.UtcNow.AddDays(1),
            Status = RequestStatus.Pending
        };
        db.ReportRequests.Add(request);
        await db.SaveChangesAsync();
        var req = await db.ReportRequests.FirstOrDefaultAsync(x => x.Id == request.Id);
        req.Status = RequestStatus.Processing;
        var events = await db.Events
            .Where(e =>
                e.ProductId == req.ProductId &&
                e.CheckoutId == req.CheckoutId &&
                e.EventTime >= req.DateFrom &&
                e.EventTime <= req.DateTo)
            .ToListAsync();
        var views = events.Count(e => e.EventType == EventType.View);
        var purchases = events.Count(e => e.EventType == EventType.Purchase);
        var ratio = views == 0 ? 0 : (decimal)purchases / views;
        var report = new Report {
            Id = Guid.NewGuid(),
            ProductId = req.ProductId,
            CheckoutId = req.CheckoutId,
            DateFrom = req.DateFrom,
            DateTo = req.DateTo,
            ViewsCount = views,
            PurchasesCount = purchases,
            Ratio = ratio,
            CreatedAt = DateTime.UtcNow
        };
        db.Reports.Add(report);
        await db.SaveChangesAsync();
        req.Status = RequestStatus.Done;
        req.ReportId = report.Id;
        await db.SaveChangesAsync();
        var savedReport = await db.Reports.FirstAsync();
        var updatedRequest = await db.ReportRequests.FirstAsync();
        savedReport.ViewsCount.Should().Be(2);
        savedReport.PurchasesCount.Should().Be(1);
        savedReport.Ratio.Should().BeApproximately(0.5m, 0.001m);
        updatedRequest.Status.Should().Be(RequestStatus.Done);
        updatedRequest.ReportId.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldFailSafelyWhenNoViews() {
        var db = CreateDb();
        var productId = Guid.NewGuid();
        var checkoutId = Guid.NewGuid();
        db.Events.Add(
            new Event {
                Id = Guid.NewGuid(),
                ProductId = productId,
                CheckoutId = checkoutId,
                EventType = EventType.Purchase,
                EventTime = DateTime.UtcNow
            });
        var request = new ReportRequest {
            Id = Guid.NewGuid(),
            ProductId = productId,
            CheckoutId = checkoutId,
            DateFrom = DateTime.UtcNow.AddDays(-1),
            DateTo = DateTime.UtcNow.AddDays(1),
            Status = RequestStatus.Pending
        };
        db.ReportRequests.Add(request);
        await db.SaveChangesAsync();
        var req = await db.ReportRequests.FirstAsync();
        req.Status = RequestStatus.Processing;
        var events = await db.Events
            .Where(e =>
                e.ProductId == req.ProductId &&
                e.CheckoutId == req.CheckoutId &&
                e.EventTime >= req.DateFrom &&
                e.EventTime <= req.DateTo)
            .ToListAsync();
        var views = events.Count(e => e.EventType == EventType.View);
        var purchases = events.Count(e => e.EventType == EventType.Purchase);
        var ratio = views == 0 ? 0 : (decimal)purchases / views;
        var report = new Report {
            Id = Guid.NewGuid(),
            ProductId = req.ProductId,
            CheckoutId = req.CheckoutId,
            DateFrom = req.DateFrom,
            DateTo = req.DateTo,
            ViewsCount = views,
            PurchasesCount = purchases,
            Ratio = ratio,
            CreatedAt = DateTime.UtcNow
        };
        db.Reports.Add(report);
        req.Status = RequestStatus.Done;
        req.ReportId = report.Id;
        await db.SaveChangesAsync();
        var savedReport = await db.Reports.FirstAsync();
        savedReport.ViewsCount.Should().Be(0);
        savedReport.Ratio.Should().Be(0);
    }
}