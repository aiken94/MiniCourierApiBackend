using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using CourierBackend.Data;
using CourierBackend.Data.Enums;
using CourierBackend.Data.Requests;
using CourierBackend.Data.Resources;
using CourierBackend.Services.Model.Interfaces;
using CourierBackend.Services.Auth.Interfaces;
using CourierBackend.Models;

namespace CourierBackend.Services.Model;

public class DashboardService : IDashboardService
{
    private readonly CourierContext _context;
    private readonly IMemoryCache _cache;

    private readonly ICurrentAdminService _currentUser;

    public DashboardService(CourierContext context, IMemoryCache cache, ICurrentAdminService currentUser)
    {
        _context = context;
        _cache = cache;
        _currentUser = currentUser;
    }

    public async Task<DashboardResource> GetDashboardAsync(DashboardRequest request)
    {
        var cacheKey = GenerateCacheKey(request);
        var adminId = _currentUser.GetId();
        String? adminRole = _currentUser.GetRole()?.ToLower();

        if (_cache.TryGetValue(cacheKey, out DashboardResource? cached))
        {
            return cached!;
        }

        var packages = _context.Packages.AsQueryable();
        var histories = _context.PackageDeliveryHistories.AsQueryable();

        // only select packages associated with authenticated user if the authenticated user is not an admin
        if (adminRole != "admin")
        {
            packages = packages.Where(p => p.AdminId == adminId);
            histories = histories.Include(p => p.Package)
                .Where(p => p.Package.AdminId == adminId);
        }

        // Apply date filters
        if (request.FromDate.HasValue)
        {
            packages = packages.Where(p => p.CreatedAt >= request.FromDate.Value);
            histories = histories.Where(h => h.Date >= DateOnly.FromDateTime(request.FromDate.Value));
        }

        if (request.ToDate.HasValue)
        {
            var toDateEnd = request.ToDate.Value.Date.AddDays(1);
            packages = packages.Where(p => p.CreatedAt < toDateEnd);
            histories = histories.Where(h => h.Date <= DateOnly.FromDateTime(request.ToDate.Value));
        }

        // Run queries sequentially — EF Core does not support parallel operations on the same DbContext
        var totalPackages = await packages.CountAsync();
        var totalAdmins = 0;

        if (adminRole == "admin")
        {
            var admins = _context.Admins.AsQueryable();
            totalAdmins = await admins.CountAsync();
        }

        var totalRevenue = await packages.SumAsync(p => (decimal?)p.Cost) ?? 0m;
        var totalHistories = await histories.CountAsync();
        var totalTrackingViews = await packages.SumAsync(p => (int?)p.NoOfTracking) ?? 0;

        var packagesPending = await packages.CountAsync(p => p.Status == PackageStatus.Pending);
        var packagesInTransit = await packages.CountAsync(p => p.Status == PackageStatus.InTransit);
        var packagesDelivered = await packages.CountAsync(p => p.Status == PackageStatus.Delivered);
        var packagesFailed = await packages.CountAsync(p => p.Status == PackageStatus.Failed);

        var recentPackageList = await packages
            .Include(p => p.Admin)
            .Include(p => p.Sender)
            .Include(p => p.Receiver)
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .ToListAsync();

        // Revenue grouped by month/year
        var revenueGrouped = await packages
            .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Revenue = g.Sum(p => p.Cost),
                PackageCount = g.Count()
            })
            .OrderByDescending(r => r.Year)
            .ThenByDescending(r => r.Month)
            .Take(12)
            .ToListAsync();

        var recentPackages = recentPackageList
            .Select(PackageResource.FromModel)
            .ToList();

        var revenueOverTime = revenueGrouped
            .Select(r => new MonthlyRevenue
            {
                Year = r.Year,
                Month = new DateTime(r.Year, r.Month, 1).ToString("MMMM"),
                Revenue = r.Revenue,
                PackageCount = r.PackageCount
            })
            .ToList();

        var statusBreakdown = new List<StatusBreakdown>
        {
            new() { Status = PackageStatus.Pending.ToString(), Count = packagesPending },
            new() { Status = PackageStatus.InTransit.ToString(), Count = packagesInTransit },
            new() { Status = PackageStatus.Delivered.ToString(), Count = packagesDelivered },
            new() { Status = PackageStatus.Failed.ToString(), Count = packagesFailed }
        };

        var result = new DashboardResource
        {
            Summary = new SummaryStats
            {
                TotalPackages = totalPackages,
                TotalAdmins = totalAdmins,
                TotalRevenue = totalRevenue,
                TotalDeliveryHistories = totalHistories,
                TotalTrackingViews = totalTrackingViews,
                PackagesPending = packagesPending,
                PackagesInTransit = packagesInTransit,
                PackagesDelivered = packagesDelivered,
                PackagesFailed = packagesFailed
            },
            StatusBreakdown = statusBreakdown,
            RecentPackages = recentPackages,
            RevenueOverTime = revenueOverTime
        };

        // Cache for 60 seconds
        _cache.Set(cacheKey, result, TimeSpan.FromSeconds(60));

        return result;
    }

    private static string GenerateCacheKey(DashboardRequest request)
    {
        return $"Dashboard_{request.FromDate?.Ticks ?? 0}_{request.ToDate?.Ticks ?? 0}";
    }
}
