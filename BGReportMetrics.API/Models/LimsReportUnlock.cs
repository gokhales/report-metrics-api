using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BGReportMetrics.API.Models;

[Table("LimsReportUnlocks")]
public class LimsReportUnlock
{
    [Key] public int Id { get; set; }
    public int ReportId { get; set; }
    public string ReportUnlockType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public int IsOnReport { get; set; } = 1;

    [ForeignKey(nameof(ReportId))]
    public LimsReport? Report { get; set; }
}
