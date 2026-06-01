namespace CourierBackend.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class PasswordResetToken
    {
        public Guid Id { get; set; }

        public int AdminId { get; set; }

        public Admin Admin { get; set; } = default!;

        public string TokenHash { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}