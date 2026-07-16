using CourierBackend.Data.Enums;

namespace CourierBackend.Data.Resources
{
    using CourierBackend.Models;

    public class PackageResource
    {
        public int Id { get; set; }

        public required AdminResource Admin { get; set; }

        public required SenderResource Sender { get; set; }

        public required ReceiverResource Receiver { get; set; }

        public required string TrackingNumber { get; set; }

        public decimal Weight { set; get; }

        public decimal Value { get; set; }

        public decimal Cost { get; set; }

        public required string Description { get; set; }

        public required DateOnly DeliveryDate { get; set; }

        public required string ImageUrl { get; set; }

        public int Tracks { get; set; }

        public PackageStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public static PackageResource FromModel(Package package)
        {
            return new PackageResource
            {
                Id = package.Id,
                Admin = AdminResource.FromModel(package.Admin),
                Sender = SenderResource.FromModel(package.Sender),
                Receiver = ReceiverResource.FromModel(package.Receiver),
                TrackingNumber = package.TrackingNumber,
                Weight = package.Weight,
                Value = package.Value,
                Cost = package.Cost,
                Description = package.Description,
                DeliveryDate = package.DeliveryDate,
                ImageUrl = package.ImageUrl,
                Tracks = package.NoOfTracking,
                Status = package.Status,
                CreatedAt = package.CreatedAt,
                UpdatedAt = package.UpdatedAt
            };
        }
    }
}