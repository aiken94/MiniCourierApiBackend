namespace CourierBackend.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class Package
    {
        public int Id { get; set; }

        public int AdminId { get; set; }

        public decimal Value { get; set; }

        public decimal Weight { get; set; }

        public required decimal Cost { get; set; }

        public string? ImageUrl { get; set; }

        public string? Description { get; set; }

        public required string TrackingNumber { get; set; }

        public DateTime DeliveryDate { get; set; }

        public int NoOfTracking { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("AdminId")]
        public Admin? Admin { get; set; }

        public List<PackageDeliveryHistory>? Histories { get; set; }
    }
}