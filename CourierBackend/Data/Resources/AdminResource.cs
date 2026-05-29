namespace CourierBackend.Data.Resources
{
    using CourierBackend.Models;

    public class AdminResource
    {
        public int Id { get; set; }
        public string? Role { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static AdminResource FromModel(Admin admin)
        {
            string[] roleType = ["Admin", "User"];

            return new AdminResource
            {
                Id = admin.Id,
                Role = roleType[admin.Role],
                Name = admin.Name,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                CreatedAt = admin.CreatedAt,
                UpdatedAt = admin.UpdatedAt
            };
        }
    }
}