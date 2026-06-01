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

        public required string ImageUrl { get; set; }

        public required string Description { get; set; }

        public required string TrackingNumber { get; set; }

        public DateOnly DeliveryDate { get; set; }

        public int NoOfTracking { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("AdminId")]
        public Admin? Admin { get; set; }

        public Receiver? Receiver { get; set; }

        public Sender? Sender { get; set; }

        public ICollection<PackageDeliveryHistory>? Histories { get; set; }
    }
}