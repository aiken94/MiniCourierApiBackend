namespace CourierBackend.Data.Resources;

using CourierBackend.Data.Enums;

public class DashboardResource
{
    public SummaryStats Summary { get; set; } = default!;
    public List<StatusBreakdown> StatusBreakdown { get; set; } = new();
    public List<PackageResource> RecentPackages { get; set; } = new();
    public List<MonthlyRevenue> RevenueOverTime { get; set; } = new();
}

public class SummaryStats
{
    public int TotalPackages { get; set; }
    public int TotalAdmins { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalDeliveryHistories { get; set; }
    public int TotalTrackingViews { get; set; }
    public int PackagesPending { get; set; }
    public int PackagesInTransit { get; set; }
    public int PackagesDelivered { get; set; }
    public int PackagesFailed { get; set; }
}

public class StatusBreakdown
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class MonthlyRevenue
{
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Revenue { get; set; }
    public int PackageCount { get; set; }
}
