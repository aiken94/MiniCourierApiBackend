using CourierBackend.Data.Requests;
using CourierBackend.Models;
using CourierBackend.Exceptions;
using CourierBackend.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using CourierBackend.Services.Model.Interfaces;

namespace CourierBackend.Services.Model;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;

    public AdminService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<Admin> GetByIdAsync(int id)
    {
        var admin = await _adminRepository.GetByIdAsync(id);

        if (admin is null)
        {
            throw new KeyNotFoundException("Admin not found");
        }

        return admin;
    }

    public async Task<Admin> RegisterAsync(AdminRequest request)
    {
        // BUSINESS VALIDATION

        var emailExists = await _adminRepository
            .EmailExistsAsync(request.Email);

        if (emailExists)
        {
            throw new BadRequestException(
                "Email already exists"
            );
        }

        PasswordHasher<AdminRequest> _passwordHasher = new PasswordHasher<AdminRequest>();

        // CREATE USER

        var admin = new Admin
        {
            Role = request.Role,
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = _passwordHasher.HashPassword(request, request.Password)
        };

        await _adminRepository.CreateAsync(admin);

        return admin;
    }

    public async Task<Admin> UpdatedAsync(AdminRequest request, int id)
    {
        var emailExists = await _adminRepository
                    .EmailUpdateExistsAsync(request.Email, id);

        if (emailExists)
        {
            throw new BadRequestException(
                "Email already exists"
            );
        }

        // Update the Admin records
        var admin = await GetByIdAsync(id);

        // Hash password
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            PasswordHasher<AdminRequest> _passwordHasher = new PasswordHasher<AdminRequest>();

            admin.PasswordHash = _passwordHasher.HashPassword(request, request.Password);
        }

        // set the new records
        admin.Role = request.Role;
        admin.Name = request.Name;
        admin.Email = request.Email;
        admin.PhoneNumber = request.PhoneNumber;

        await _adminRepository.UpdateAsync(admin);

        return admin;
    }

    public async Task DeleteAsync(int id)
    {
        var admin = await GetByIdAsync(id);

        await _adminRepository.DeleteAsync(admin);
    }
}