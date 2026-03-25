namespace CourierBackend.Data
{
    using Microsoft.EntityFrameworkCore;
    using CourierBackend.Models;

    public class CourierContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; }

        public DbSet<PackageDeliveryHistory> PackageDeliveryHistories { get; set; }

        public DbSet<Package> Packages { get; set; }

        public DbSet<Receiver> Receivers { get; set; }

        public DbSet<Sender> Senders { get; set; }
    }
}