using BGReportMetrics.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BGReportMetrics.API.Data;

/// <summary>MGL database — Samples and all related MGL tables.</summary>
public class MglDbContext(DbContextOptions<MglDbContext> options) : DbContext(options)
{
    public DbSet<Sample> Samples => Set<Sample>();
    public DbSet<ArchiveReportLog> ArchiveReportLog => Set<ArchiveReportLog>();
    public DbSet<SamplesAdditionalInfo> SamplesAdditionalInfo => Set<SamplesAdditionalInfo>();
    public DbSet<ReportUnlock> ReportUnlocks => Set<ReportUnlock>();
    public DbSet<AllTestCode> AllTestCodes => Set<AllTestCode>();
    public DbSet<ClientTestCode> ClientTestCodes => Set<ClientTestCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Sample>().HasIndex(s => s.FaxedDate);
        modelBuilder.Entity<Sample>().HasIndex(s => s.BillType);
        modelBuilder.Entity<Sample>().HasIndex(s => s.TestCode);

        modelBuilder.Entity<SamplesAdditionalInfo>()
            .HasOne(s => s.Sample)
            .WithOne(s => s.AdditionalInfo)
            .HasForeignKey<SamplesAdditionalInfo>(s => s.SampleId);
    }
}
