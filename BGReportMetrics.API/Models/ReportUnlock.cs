using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BGReportMetrics.API.Models;

[Table("ReportUnlocks")]
public class ReportUnlock
{
    [Key] public int Id { get; set; }
    public int SampleId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ReportUnlockType { get; set; } = string.Empty;

    [ForeignKey(nameof(SampleId))]
    public Sample? Sample { get; set; }
}
