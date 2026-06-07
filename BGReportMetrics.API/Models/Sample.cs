using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BGReportMetrics.API.Models;

[Table("Samples")]
public class Sample
{
    [Key] public int Id { get; set; }
    public string LabNumber { get; set; } = string.Empty;
    public string TestCode { get; set; } = string.Empty;
    public DateTime? FaxedDate { get; set; }
    public DateTime? OriginalFaxDate { get; set; }
    public string BillType { get; set; } = string.Empty;
    public int ManualClose { get; set; } = 0;
    public string? Interpretation { get; set; }
    public DateTime? TestOrderDate { get; set; }
    public int? ClientTestCodeId { get; set; }

    [ForeignKey(nameof(ClientTestCodeId))]
    public ClientTestCode? ClientTestCode { get; set; }
    public SamplesAdditionalInfo? AdditionalInfo { get; set; }
    public ICollection<ArchiveReportLog> ArchiveLogs { get; set; } = new List<ArchiveReportLog>();
    public ICollection<ReportUnlock> ReportUnlocks { get; set; } = new List<ReportUnlock>();
}
