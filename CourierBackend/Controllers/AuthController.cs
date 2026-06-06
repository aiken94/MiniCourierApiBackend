namespace CourierBackend.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using CourierBackend.Data;
    using Microsoft.AspNetCore.Mvc;
    using CourierBackend.Data.Requests.Auth;
    using Microsoft.AspNetCore.Identity;
    using CourierBackend.Services.Auth.Interfaces;
    using CourierBackend.Helpers;
    using Microsoft.EntityFrameworkCore;
    using FluentValidation;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly CourierContext _context;
        private readonly IValidator<ForgotPasswordRequest> _forgotPasswordValidator;
        private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;
        public AuthController(CourierContext context, IJwtService jwtService, IValidator<ForgotPasswordRequest> forgotPasswordValidator, IValidator<ResetPasswordRequest> resetPasswordValidator)
        {
            _context = context;
            _jwtService = jwtService;
            _forgotPasswordValidator = forgotPasswordValidator;
            _resetPasswordValidator = resetPasswordValidator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (admin is null)
            {
                return Unauthorized(ResponseStructures._401Response("Invalid credentials"));
            }

            PasswordHasher<LoginRequest> _passwordHasher = new PasswordHasher<LoginRequest>();

            var verificationResult = _passwordHasher.VerifyHashedPassword(request, admin.PasswordHash, request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(ResponseStructures._401Response("Invalid credentials"));
            }

            //if (!admin.IsActive)
            //{
            //    return Unauthorized(ResponseStructures._401Response("Admin account is inactive"));
            //}

            var response = await _jwtService.GenerateTokensAsync(admin);

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _jwtService.RefreshTokenAsync(request.RefreshToken);

            if (response is null)
            {
                return Unauthorized(ResponseStructures._401Response("Invalid refresh token"));
            }

            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _forgotPasswordValidator.ValidateAsync(request);

            if (!result.IsValid)
            {
                return BadRequest(ResponseStructures.ErrorResponse(result.Errors.ToDictionary()));
            }

            await _jwtService.ForgotPasswordAsync(request.Email);

            return Ok(ResponseStructures.EmptyOkResponse("If the email exists, a reset link has been sent."));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _resetPasswordValidator.ValidateAsync(request);

            if (!result.IsValid)
            {
                return BadRequest(ResponseStructures.ErrorResponse(result.Errors.ToDictionary()));
            }

            try
            {
                await _jwtService.ResetPasswordAsync(request.Token, request.NewPassword);

                return Ok(ResponseStructures.EmptyOkResponse("Password reset successful."));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseStructures._400Response(ex.Message));
            }
        }
    }
}