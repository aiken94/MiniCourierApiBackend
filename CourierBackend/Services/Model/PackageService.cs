using CourierBackend.Data.Requests;
using CourierBackend.Models;
using CourierBackend.Data.Repositories.Interfaces;
using CourierBackend.Services.Model.Interfaces;
using CourierBackend.Data.Resources;
using Microsoft.AspNetCore.Mvc;

namespace CourierBackend.Services.Model;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _packageRepository;

    private readonly IQueryService _queryService;

    public PackageService(IPackageRepository packageRepository, IQueryService queryService)
    {
        _packageRepository = packageRepository;
        _queryService = queryService;
    }

    public async Task<Pagination<PackageResource>> GetPackagesAsync([FromQuery] QueryParameters parameters)
    {
        var query = _packageRepository.GetPackagesAsync();

        return await _queryService.QueryAsync(
            query,
            parameters,
            PackageResource.FromModel,
            // SEARCH LOGIC
            (q, search) => q.Where(p =>
                p.TrackingNumber.ToLower().Contains(search.ToLower()) ||
                p.Description.ToLower().Contains(search.ToLower())
            )
        );
    }

    public async Task<Package?> GetByIdAsync(int id)
    {
        return await _packageRepository.GetByIdAsync(id);
    }

    public async Task<Package?> CreateAsync(PackageRequest request)
    {
        Package package = await _packageRepository.CreateAsync(request);

        return package;
    }

    public async Task<Package?> UpdateAsync(PackageRequest request, Package package)
    {
        // if it exists, update it
        return await _packageRepository.UpdateAsync(request, package);
    }

    public async Task DeleteAsync(Package package)
    {
        await _packageRepository.DeleteAsync(package);
    }

    public async Task<Package?> GetByTrackingNumberAsync(string trackingNumber)
    {
        return await _packageRepository.GetByTrackingNumberAsync(trackingNumber);
    }
}