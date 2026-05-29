namespace CourierBackend.Data.Repositories.Interfaces
{
    using CourierBackend.Models;

    public interface IAdminRepository
    {
        Task<Admin?> GetByIdAsync(int id);

        Task<bool> EmailExistsAsync(string email);

        Task<bool> EmailUpdateExistsAsync(string email, int id);

        Task CreateAsync(Admin admin);

        Task UpdateAsync(Admin admin);

        Task DeleteAsync(Admin admin);
    }
}