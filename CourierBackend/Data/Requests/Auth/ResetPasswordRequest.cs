namespace CourierBackend.Data.Requests.Auth
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; } = null!;

        public string NewPassword { get; set; } = null!;

        public string ConfirmPassword { get; set; } = null!;
    }
}