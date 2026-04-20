namespace CourierBackend.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;

    [Index(nameof(Email), IsUnique = true)]
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        public enum RoleType
        {
            Admin,
            User
        }

        [DefaultValue(RoleType.User)]
        public RoleType Role { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string PhoneNumber { get; set; }

        public required string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}