namespace CourierBackend.Data.Resources
{
    using CourierBackend.Models;

    public class TrackingResource
    {
        public int Id { get; set; }

        public required SenderResource Sender { get; set; }

        public required ReceiverResource Receiver { get; set; }

        public required ICollection<DeliveryHistoryResource> Histories { get; set; }

        public required string TrackingNumber { get; set; }

        public decimal Weight { set; get; }

        public decimal Value { get; set; }

        public decimal Cost { get; set; }

        public required string Description { get; set; }

        public required DateOnly DeliveryDate { get; set; }

        public required string ImageUrl { get; set; }

        public int Tracks { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public static TrackingResource FromModel(Package package)
        {
            return new TrackingResource
            {
                Id = package.Id,
                Sender = SenderResource.FromModel(package.Sender),
                Receiver = ReceiverResource.FromModel(package.Receiver),
                Histories = package.Histories?
                    .OrderByDescending(x => x.Id)
                    .Select(DeliveryHistoryResource.FromModel)
                    .ToList()
                    ?? new List<DeliveryHistoryResource>(),
                TrackingNumber = package.TrackingNumber,
                Weight = package.Weight,
                Value = package.Value,
                Cost = package.Cost,
                Description = package.Description,
                DeliveryDate = package.DeliveryDate,
                ImageUrl = package.ImageUrl,
                Tracks = package.NoOfTracking,
                CreatedAt = package.CreatedAt,
                UpdatedAt = package.UpdatedAt
            };
        }
    }
}