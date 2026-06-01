namespace CourierBackend.Data.Repositories.Interfaces
{
    using CourierBackend.Data.Requests;
    using CourierBackend.Models;

    public interface IPackageRepository
    {
        IQueryable<Package> GetPackagesAsync();

        Task<Package?> GetByIdAsync(int id);

        Task<Package> CreateAsync(PackageRequest request);

        Task<Package> UpdateAsync(PackageRequest request, Package package);

        Task DeleteAsync(Package package);

        Task<Package?> GetByTrackingNumberAsync(string trackingNumber);
    }
}