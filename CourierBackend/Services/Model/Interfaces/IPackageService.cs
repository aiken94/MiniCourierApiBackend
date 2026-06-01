namespace CourierBackend.Services.Model.Interfaces;

using CourierBackend.Data.Requests;
using CourierBackend.Data.Resources;
using CourierBackend.Models;

public interface IPackageService
{
    Task<Pagination<PackageResource>> GetPackagesAsync(QueryParameters parameters);

    Task<Package?> GetByIdAsync(int id);

    Task<Package?> CreateAsync(PackageRequest request);

    Task<Package?> UpdateAsync(PackageRequest request, Package package);

    Task DeleteAsync(Package package);

    Task<Package?> GetByTrackingNumberAsync(string trackingNumber);
}