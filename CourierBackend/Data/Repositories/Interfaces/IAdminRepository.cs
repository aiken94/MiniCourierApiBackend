namespace CourierBackend.Data.Repositories.Interfaces
{
    using CourierBackend.Models;

    public interface IAdminRepository
    {
        Task<bool> EmailExistsAsync(string email);

        Task CreateAsync(Admin admin);
    }
}