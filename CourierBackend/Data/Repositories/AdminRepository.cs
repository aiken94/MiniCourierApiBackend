namespace CourierBackend.Data.Repositories
{
    using System.Threading.Tasks;
    using CourierBackend.Models;
    using Microsoft.EntityFrameworkCore;
    using CourierBackend.Data;
    using CourierBackend.Data.Repositories.Interfaces;

    public class AdminRepository : IAdminRepository
    {
        private readonly CourierContext _context;

        public AdminRepository(CourierContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Admins
                .AnyAsync(x => x.Email == email);
        }

        public async Task CreateAsync(Admin admin)
        {
            await _context.Admins.AddAsync(admin);

            await _context.SaveChangesAsync();
        }
    }
}