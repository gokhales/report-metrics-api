using BGReportMetrics.API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BGReportMetrics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaticController : ControllerBase
{
    [HttpGet("white-label-customers")]
    public IActionResult WhiteLabelCustomers() =>
        Ok(new[] { "Natera", "Quest", "GH", "BTOO", "CUHK", "MITE", "ENZO", "SONIC" }
            .Select(n => new StaticListDto(n)));

    [HttpGet("discrete-data-customers")]
    public IActionResult DiscreteDataCustomers() =>
        Ok(new[] { "Natera", "Quest", "BTO", "SONIC", "GH", "NIAID" }
            .Select(n => new StaticListDto(n)));

    [HttpGet("data-delivery-customers")]
    public IActionResult DataDeliveryCustomers() =>
        Ok(new[] { "VA", "GH", "Natera (Post June)", "IMH", "CHOP", "NIAID", "2 EPIC Clients" }
            .Select(n => new StaticListDto(n)));

    [HttpGet("systems")]
    public IActionResult Systems() =>
        Ok(new[] { "RARE", "LIS BIO", "LIS PGx", "Cytolink", "MitoLink", "BioLink", "DNABler" }
            .Select(n => new StaticListDto(n)));

    [HttpGet("result-access")]
    public IActionResult ResultAccess() =>
        Ok(new[] { "Email", "Fax", "Portal v1 and V2", "Dex", "EPIC", "GeneResults", "TCHOutbound", "VistaraOutbound" }
            .Select(n => new StaticListDto(n)));
}
