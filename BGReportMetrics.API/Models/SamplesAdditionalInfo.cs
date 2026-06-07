using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BGReportMetrics.API.Models;

[Table("SamplesAdditionalInfo")]
public class SamplesAdditionalInfo
{
    [Key] public int Id { get; set; }
    public int SampleId { get; set; }
    public int WasAutoSignedOut { get; set; } = 0;
    public int UpgradedOrder { get; set; } = 0;
    public DateTime? DueDate { get; set; }

    [ForeignKey(nameof(SampleId))]
    public Sample? Sample { get; set; }
}
