using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BGReportMetrics.API.Models;

[Table("ArchiveReportLog")]
public class ArchiveReportLog
{
    [Key] public int Id { get; set; }
    public int SamplesId { get; set; }
    public DateTime WhenArchived { get; set; }

    [ForeignKey(nameof(SamplesId))]
    public Sample? Sample { get; set; }
}
