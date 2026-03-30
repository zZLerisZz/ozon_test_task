using Domain.Enums;
using Infrastructure.Kafka;
using Microsoft.EntityFrameworkCore;
using OzonTestTask.Infrastructure;
using ReportWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"), o => {
        o.MapEnum<RequestStatus>("report_request_status");
        o.MapEnum<EventType>("event_type");
        }));
builder.Services.AddSingleton<KafkaConsumer>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
