using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BGReportMetrics.API.Models;

[Table("AllTestCodes")]
public class AllTestCode
{
    [Key] public int Id { get; set; }
    public string TestCode { get; set; } = string.Empty;
    public int TAT { get; set; } // turnaround time in days
}
