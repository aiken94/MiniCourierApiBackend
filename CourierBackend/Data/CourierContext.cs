namespace CourierBackend.Data
{
    using Microsoft.EntityFrameworkCore;
    using CourierBackend.Models;
    using CourierBackend.Configurations.Models;

    public class CourierContext : DbContext
    {
        public CourierContext(DbContextOptions<CourierContext> options) : base(options)
        {

        }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<PackageDeliveryHistory> PackageDeliveryHistories { get; set; }

        public DbSet<Package> Packages { get; set; }

        public DbSet<Receiver> Receivers { get; set; }

        public DbSet<Sender> Senders { get; set; }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        public DbSet<PersonalAccessToken> PersonalAccessTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // apply all configurations from the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourierContext).Assembly);

            modelBuilder.Entity<PersonalAccessToken>()
            .HasIndex(r => r.Token)
            .IsUnique();

            modelBuilder.Entity<Package>()
                .HasOne(p => p.Admin)
                .WithMany(a => a.Packages)
                .HasForeignKey(p => p.AdminId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Package>()
                .HasOne(p => p.Sender)
                .WithOne(s => s.Package)
                .HasForeignKey<Sender>(s => s.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Package>()
                .HasOne(p => p.Receiver)
                .WithOne(r => r.Package)
                .HasForeignKey<Receiver>(r => r.PackageId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Package>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<PackageDeliveryHistory>()
                .HasOne(p => p.Package)
                .WithMany(p => p.Histories)
                .HasForeignKey(p => p.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PasswordResetToken>()
                .HasIndex(x => x.TokenHash);
        }
    }
}