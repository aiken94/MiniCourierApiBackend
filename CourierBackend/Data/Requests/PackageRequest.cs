using System.Reflection.Metadata;

namespace CourierBackend.Data.Requests
{
    public class PackageRequest
    {
        public required string SenderName { get; set; }

        public required string SenderEmail { get; set; }

        public required string SenderPhoneNumber { get; set; }

        public required string SenderCountry { get; set; }

        public required string SenderAddress { get; set; }

        public required string ReceiverName { get; set; }

        public required string ReceiverEmail { get; set; }

        public required string ReceiverPhoneNumber { get; set; }

        public required string ReceiverCountry { get; set; }

        public required string ReceiverAddress { get; set; }

        public required decimal PackageWeight { get; set; }

        public required decimal PackageCost { get; set; }

        public required decimal PackageValue { get; set; }

        public required string PackageDescription { get; set; }

        public required DateOnly EstimatedDeliveryDate { get; set; }

        public required IFormFile PackageImage { get; set; }
    }
}