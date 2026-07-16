namespace CourierBackend.Data.Requests;

public class DashboardRequest
{
    /// <summary>
    /// Filter packages created on or after this date.
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Filter packages created on or before this date.
    /// </summary>
    public DateTime? ToDate { get; set; }
}
