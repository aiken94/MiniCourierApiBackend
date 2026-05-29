namespace CourierBackend.Data.Resources
{
    using CourierBackend.Models;

    public class SenderResource
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }

        public static SenderResource FromModel(Sender sender)
        {
            string[] roleType = ["Admin", "User"];

            return new SenderResource
            {
                Id = sender.Id,
                Name = sender.Name,
                Email = sender.Email,
                PhoneNumber = sender.PhoneNumber,
                Country = sender.Country,
                Address = sender.Address
            };
        }
    }
}