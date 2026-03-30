using Infrastructure.Kafka;
using Microsoft.EntityFrameworkCore;
using OzonTestTask.Infrastructure;
using Domain.Enums;
using OzonTestTask.Domain;

namespace ReportWorker {
    /// <summary>
    /// Worker, который отвечает за обработку создаваемых запросов
    /// </summary>
    public class Worker: BackgroundService {
        private readonly KafkaConsumer _consumer;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(KafkaConsumer consumer, IServiceScopeFactory scopeFactory) {
            _consumer = consumer;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            await _consumer.ConsumeAsync(async message => {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var request = await db.ReportRequests.FirstOrDefaultAsync(x => x.Id == message.RequestId);
                if(request == null)
                    return;
                try {
                    request.Status = RequestStatus.Processing;
                    await db.SaveChangesAsync();
                    var events = await db.Events.Where(e =>
                        e.ProductId == request.ProductId &&
                        e.CheckoutId == request.CheckoutId &&
                        e.EventTime >= request.DateFrom &&
                        e.EventTime <= request.DateTo).ToListAsync();
                    var views = events.Count(e => e.EventType == EventType.View);
                    var purchases = events.Count(e => e.EventType == EventType.Purchase);
                    var ratio = views == 0 ? 0 : (decimal)purchases / views;
                    var report = new Report {
                        Id = Guid.NewGuid(),
                        ProductId = request.ProductId,
                        CheckoutId = request.CheckoutId,
                        DateFrom = request.DateFrom,
                        DateTo = request.DateTo,
                        ViewsCount = views,
                        PurchasesCount = purchases,
                        Ratio = ratio,
                        CreatedAt = DateTime.UtcNow
                    };
                    db.Reports.Add(report);
                    await db.SaveChangesAsync();
                    request.Status = RequestStatus.Done;
                    request.ReportId = report.Id;
                    await db.SaveChangesAsync();
                }
                catch {
                    request.Status = RequestStatus.Failed;
                    await db.SaveChangesAsync();
                }
            }, stoppingToken);
        }
    }
}
