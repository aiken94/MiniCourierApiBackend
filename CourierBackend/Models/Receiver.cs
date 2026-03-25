namespace CourierBackend.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Receiver
    {
        [Key]
        public int Id { get; set; }

        public int PackageId { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string PhoneNumber { get; set; }

        public required string Country { get; set; }

        public string Address { get; set; } = string.Empty;

        [ForeignKey("PackageId")]
        public Package? Package { get; set; }
    }
}