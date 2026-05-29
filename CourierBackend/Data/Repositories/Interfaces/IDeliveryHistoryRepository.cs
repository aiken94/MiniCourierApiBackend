namespace CourierBackend.Data.Repositories.Interfaces
{
    using CourierBackend.Models;
    using CourierBackend.Data.Resources;

    public interface IDeliveryHistoryRepository
    {
        Task<List<DeliveryHistoryResource>> GetHistoriesAsync(int package_id);

        Task<PackageDeliveryHistory> GetHistoryByIdAsync(int id);

        Task<PackageDeliveryHistory> CreateAsync(PackageDeliveryHistory history);

        Task DeleteAsync(PackageDeliveryHistory history);
    }
}