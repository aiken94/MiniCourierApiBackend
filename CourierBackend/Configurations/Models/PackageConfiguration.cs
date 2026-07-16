using CourierBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierBackend.Configurations.Models;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.HasOne(p => p.Sender)
            .WithOne(s => s.Package)
            .HasForeignKey<Sender>(s => s.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Receiver)
            .WithOne(r => r.Package)
            .HasForeignKey<Receiver>(r => r.PackageId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(p => p.Status)
            .HasConversion<string>();
    }
}