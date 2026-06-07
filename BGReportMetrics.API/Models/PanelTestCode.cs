using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BGReportMetrics.API.Models;

[Table("PanelTestCode")]
public class PanelTestCode
{
    [Key] public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<LimsOrder> Orders { get; set; } = new List<LimsOrder>();
}
