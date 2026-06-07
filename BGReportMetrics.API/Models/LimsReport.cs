using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BGReportMetrics.API.Models;

[Table("LimsReport")]
public class LimsReport
{
    [Key] public int Id { get; set; }
    public int OrderId { get; set; }
    public string ReportedBy { get; set; } = string.Empty;
    public DateTime ReportedDate { get; set; }

    [ForeignKey(nameof(OrderId))]
    public LimsOrder? Order { get; set; }
    public ICollection<LimsReportUnlock> ReportUnlocks { get; set; } = new List<LimsReportUnlock>();
}
