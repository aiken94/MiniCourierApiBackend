using CourierBackend.Data.Requests;
using CourierBackend.Data.Resources;

namespace CourierBackend.Services.Model.Interfaces;

public interface IDashboardService
{
    Task<DashboardResource> GetDashboardAsync(DashboardRequest request);
}
