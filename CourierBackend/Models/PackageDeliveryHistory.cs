namespace CourierBackend.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public enum LocationType
    {
        Origin,
        Destination
    }

    public class PackageDeliveryHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PackageId { get; set; }

        [Required]
        public required string Remarks { get; set; }

        [Required]
        public string? Location { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [ForeignKey("PackageId")]
        public Package? Package { get; set; }
    }
}