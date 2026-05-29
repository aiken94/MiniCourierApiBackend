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

        public async Task<bool> EmailUpdateExistsAsync(string email, int id)
        {
            // return await _context.Admins
            //     .Where(x => x.Email == email)
            //     .Where(x => x.Id != id)
            //     .AnyAsync();

            return await _context.Admins
                    .AnyAsync(x =>
                        x.Email == email &&
                        x.Id != id);
        }

        public async Task<Admin?> GetByIdAsync(int id)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateAsync(Admin admin)
        {
            await _context.Admins.AddAsync(admin);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Admin admin)
        {
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Admin admin)
        {
            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
        }
    }
}