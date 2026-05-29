namespace CourierBackend.Services.Model.Interfaces;

using CourierBackend.Data.Requests;
using CourierBackend.Models;
using CourierBackend.Data.Resources;

public interface IDeliveryHistoryService
{
    Task<List<DeliveryHistoryResource>> GetHistoriesAsync(int package_id);

    Task<PackageDeliveryHistory> CreateAsync(DeliveryHistoryRequest request);

    Task<bool> DeleteAsync(int id);
}