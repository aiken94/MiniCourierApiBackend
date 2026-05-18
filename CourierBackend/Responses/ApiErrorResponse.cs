namespace CourierBackend.Responses;

public class ApiErrorResponse
{
    public string Message { get; set; } = default!;

    public List<string>? Errors { get; set; }

    public string? Status { get; set; }
}