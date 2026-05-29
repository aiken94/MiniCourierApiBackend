namespace CourierBackend.Services.Model.Interfaces;

using CourierBackend.Data.Requests;
using CourierBackend.Models;

public interface IAdminService
{
    Task<Admin> GetByIdAsync(int id);

    Task<Admin> RegisterAsync(AdminRequest request);

    Task<Admin> UpdatedAsync(AdminRequest request, int id);

    Task DeleteAsync(int id);
}