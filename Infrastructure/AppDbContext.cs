using Domain;
using Domain.Enums;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using OzonTestTask.Domain;
using OzonTestTask.Infrastructure.Configurations;

namespace OzonTestTask.Infrastructure;
    
public class AppDbContext: DbContext {
    public DbSet<ReportRequest> ReportRequests => Set<ReportRequest>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Event> Events => Set<Event>();

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasPostgresEnum<RequestStatus>("report_request_status");
        modelBuilder.HasPostgresEnum<EventType>("event_type");

        modelBuilder.ApplyConfiguration(new ReportRequestConfiguration());
        modelBuilder.ApplyConfiguration(new ReportConfiguration());
        modelBuilder.ApplyConfiguration(new EventConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
