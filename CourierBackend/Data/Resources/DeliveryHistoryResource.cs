namespace CourierBackend.Data.Resources
{
    using CourierBackend.Models;

    public class DeliveryHistoryResource
    {
        public int Id { get; set; }
        public string? Remarks { get; set; }
        public string? Location { get; set; }
        public DateOnly Date { get; set; }

        public static DeliveryHistoryResource FromModel(PackageDeliveryHistory history)
        {
            return new DeliveryHistoryResource
            {
                Id = history.Id,
                Remarks = history.Remarks,
                Location = history.Location,
                Date = history.Date
            };
        }
    }
}