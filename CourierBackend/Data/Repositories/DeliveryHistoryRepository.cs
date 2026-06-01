namespace CourierBackend.Data.Repositories
{
    using System.Threading.Tasks;
    using CourierBackend.Models;
    using CourierBackend.Data;
    using CourierBackend.Data.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using CourierBackend.Data.Resources;

    public class DeliveryHistoryRepository : IDeliveryHistoryRepository
    {
        private readonly CourierContext _context;

        public DeliveryHistoryRepository(CourierContext context)
        {
            _context = context;
        }

        public async Task<PackageDeliveryHistory?> GetHistoryByIdAsync(int id)
        {
            return await _context.PackageDeliveryHistories.Where(history => history.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<DeliveryHistoryResource>> GetHistoriesAsync(int packageId)
        {
            return await _context.PackageDeliveryHistories
                .Where(x => x.PackageId == packageId)
                .OrderByDescending(x => x.Id)
                .Select(x => new DeliveryHistoryResource
                {
                    Id = x.Id,
                    Remarks = x.Remarks,
                    Location = x.Location,
                    Date = x.Date
                })
                .ToListAsync();
        }

        public async Task<PackageDeliveryHistory?> CreateAsync(PackageDeliveryHistory history)
        {
            await _context.PackageDeliveryHistories.AddAsync(history);

            await _context.SaveChangesAsync();

            return history;
        }

        public async Task DeleteAsync(PackageDeliveryHistory history)
        {
            _context.PackageDeliveryHistories.Remove(history);
            await _context.SaveChangesAsync();
        }
    }
}