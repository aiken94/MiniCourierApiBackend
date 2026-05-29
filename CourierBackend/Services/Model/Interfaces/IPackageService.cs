namespace CourierBackend.Services.Model.Interfaces;

using CourierBackend.Data.Requests;
using CourierBackend.Data.Resources;
using CourierBackend.Models;

public interface IPackageService
{
    Task<Pagination<PackageResource>> GetPackagesAsync(QueryParameters parameters);

    Task<Package> GetByIdAsync(int id);

    Task<Package> CreateAsync(PackageRequest request);

    Task<Package> UpdateAsync(PackageRequest request, int id);

    Task DeleteAsync(Package package);
}