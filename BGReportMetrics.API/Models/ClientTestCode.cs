using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BGReportMetrics.API.Models;

[Table("ClientTestCodes")]
public class ClientTestCode
{
    [Key] public int Id { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public int TAT { get; set; } // client-overridden TAT in days
}
