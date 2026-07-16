namespace CourierBackend.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CourierBackend.Data.Requests;
using CourierBackend.Services.Model.Interfaces;
using CourierBackend.Helpers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard([FromQuery] DashboardRequest request)
    {
        var dashboard = await _dashboardService.GetDashboardAsync(request);

        return Ok(ResponseStructures.SuccessResponse(dashboard));
    }
}
