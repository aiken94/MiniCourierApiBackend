namespace CourierBackend.Data.Requests.Auth
{
    using CourierBackend.Data.Resources;

    public class LoginResource
    {
        public AdminResource Admin { get; set; } = null!;

        public string AccessToken { get; set; } = default!;

        public string RefreshToken { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }
    }
}