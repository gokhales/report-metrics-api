using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BGReportMetrics.API.Models;

[Table("LimsOrder")]
public class LimsOrder
{
    [Key] public int Id { get; set; }
    public string OldLabNumber { get; set; } = string.Empty;
    public int PanelTestCodeId { get; set; }

    [ForeignKey(nameof(PanelTestCodeId))]
    public PanelTestCode? PanelTestCode { get; set; }
    public ICollection<LimsReport> Reports { get; set; } = new List<LimsReport>();
}
