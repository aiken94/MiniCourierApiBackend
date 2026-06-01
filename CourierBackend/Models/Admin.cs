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

        [DefaultValue(1)]
        public int Role { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string PhoneNumber { get; set; }

        public required string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DefaultValue(typeof(DateTime), "UtcNow")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Package> Packages { get; set; } = new List<Package>();

        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
    }
}