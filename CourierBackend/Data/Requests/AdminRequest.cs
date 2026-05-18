using CourierBackend.Models;

namespace CourierBackend.Data.Requests
{
    public class AdminRequest
    {
        public Admin.RoleType Role { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string PhoneNumber { get; set; }

        public required string Password { get; set; }
    }
}