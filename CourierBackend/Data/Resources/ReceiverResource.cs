namespace CourierBackend.Data.Resources
{
    using CourierBackend.Models;

    public class ReceiverResource
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }

        public static ReceiverResource FromModel(Receiver receiver)
        {
            return new ReceiverResource
            {
                Id = receiver.Id,
                Name = receiver.Name,
                Email = receiver.Email,
                PhoneNumber = receiver.PhoneNumber,
                Country = receiver.Country,
                Address = receiver.Address
            };
        }
    }
}