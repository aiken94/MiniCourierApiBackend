namespace CourierBackend.Models;

public class PersonalAccessToken
{
    public Guid Id { get; set; }

    public string Token { get; set; } = default!;

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime CreatedAt { get; set; }

    public int AdminId { get; set; }

    public Admin Admin { get; set; } = default!;
}