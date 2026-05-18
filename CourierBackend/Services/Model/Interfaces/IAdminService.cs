namespace CourierBackend.Services.Model.Interfaces;

using CourierBackend.Data.Requests;
using CourierBackend.Models;

public interface IAdminService
{
    Task<Admin> RegisterAsync(AdminRequest request);
}