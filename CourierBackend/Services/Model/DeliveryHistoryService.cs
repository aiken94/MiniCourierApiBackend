using CourierBackend.Data.Requests;
using CourierBackend.Models;
using CourierBackend.Data.Repositories.Interfaces;
using CourierBackend.Services.Model.Interfaces;
using CourierBackend.Data.Resources;

namespace CourierBackend.Services.Model;

public class DeliveryHistoryService : IDeliveryHistoryService
{
    private readonly IDeliveryHistoryRepository _deliveryHistoryRepository;

    public DeliveryHistoryService(IDeliveryHistoryRepository deliveryHistoryRepository)
    {
        _deliveryHistoryRepository = deliveryHistoryRepository;
    }

    public async Task<List<DeliveryHistoryResource>> GetHistoriesAsync(int package_id)
    {
        return await _deliveryHistoryRepository.GetHistoriesAsync(package_id);
    }

    public async Task<PackageDeliveryHistory> CreateAsync(DeliveryHistoryRequest request)
    {
        PackageDeliveryHistory history = new PackageDeliveryHistory
        {
            PackageId = request.PackageId,
            Remarks = request.Remarks,
            Location = request.Location,
            Date = request.Date
        };

        return await _deliveryHistoryRepository.CreateAsync(history);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        PackageDeliveryHistory history = await _deliveryHistoryRepository.GetHistoryByIdAsync(id);

        if (history != null)
        {
            await _deliveryHistoryRepository.DeleteAsync(history);

            return true;
        }

        else
        {
            return false;
        }
    }
}