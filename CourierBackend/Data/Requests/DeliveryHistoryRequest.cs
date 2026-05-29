using CourierBackend.Models;

namespace CourierBackend.Data.Requests
{
    public class DeliveryHistoryRequest
    {
        public required int PackageId { get; set; }

        public required string Remarks { get; set; }

        public required LocationType Location { get; set; }

        public required DateOnly Date { get; set; }
    }
}