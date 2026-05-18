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

        // Hash password
        request.Password = _passwordHasher.HashPassword(request, request.Password);

        // CREATE USER

        var admin = new Admin
        {
            Role = request.Role,
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = request.Password
        };

        await _adminRepository.CreateAsync(admin);

        return admin;
    }
}