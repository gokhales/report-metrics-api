using BGReportMetrics.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BGReportMetrics.API.Data;

/// <summary>LIMS database — PGx/BIO reports (LimsReport, LimsOrder, PanelTestCode, LimsReportUnlocks).</summary>
public class LimsDbContext(DbContextOptions<LimsDbContext> options) : DbContext(options)
{
    public DbSet<PanelTestCode> PanelTestCodes => Set<PanelTestCode>();
    public DbSet<LimsOrder> LimsOrders => Set<LimsOrder>();
    public DbSet<LimsReport> LimsReports => Set<LimsReport>();
    public DbSet<LimsReportUnlock> LimsReportUnlocks => Set<LimsReportUnlock>();
}
