namespace CourierBackend.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("Senders"), Comment("Table for storing packages sender information")]
    public class Sender
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        [Column(Order = 1)]
        public int PackageId { get; set; }

        [Column(TypeName = "varchar(255)"), Comment("Sender's name")]
        public required string Name { get; set; }

        [Column(TypeName = "varchar(255)"), Comment("Sender's email"), EmailAddress(ErrorMessage = "Invalid email address format"), MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public required string Email { get; set; }

        [Column(TypeName = "varchar(255)"), Comment("Sender's phone number")]
        public required string PhoneNumber { get; set; }

        [Column(TypeName = "varchar(100)"), Comment("Sender's country")]
        public required string Country { get; set; }

        [Column(TypeName = "text"), Comment("Sender's address")]
        public string Address { get; set; } = string.Empty;

        [ForeignKey("PackageId")]
        public Package? Package { get; set; }
    }
}