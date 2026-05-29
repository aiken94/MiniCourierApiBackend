namespace CourierBackend.Data.Requests
{
    public class AdminRequest
    {
        public required int Role { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string PhoneNumber { get; set; }

        public string? Password { get; set; }
    }
}