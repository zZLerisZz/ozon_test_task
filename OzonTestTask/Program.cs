using Domain.Enums;
using Infrastructure.Kafka;
using Microsoft.EntityFrameworkCore;
using OzonTestTask.Infrastructure;
using OzonTestTask.Services;
using OzonTestTask.Services.GRPC;

var builder = WebApplication.CreateBuilder(args); 

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"), 
    o => o.MapEnum<RequestStatus>("report_request_status")));

builder.WebHost.ConfigureKestrel(options => {
    options.ListenAnyIP(8080, listenOptions => {
        listenOptions.Protocols =
            Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});

builder.Services.AddScoped<IReportService, ReportService>(); 
builder.Services.AddScoped<IMessage, KafkaProducer>(); 
builder.Services.AddGrpc(); 
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); 

var app = builder.Build(); 
if(app.Environment.IsDevelopment()) { 
    app.UseSwagger(); 
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); 
app.UseAuthorization(); 
app.MapControllers(); 
app.MapGrpcService<ReportGrpcService>();

app.Run();