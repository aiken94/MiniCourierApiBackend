namespace CourierBackend.Data.Validators.Admin
{
    using FluentValidation;
    using CourierBackend.Models;

    public class CreateValidator : AbstractValidator<Admin>
    {
        public CreateValidator()
        {
            RuleFor(admin => admin.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(admin => admin.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

            RuleFor(admin => admin.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

            RuleFor(admin => admin.PasswordHash)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Must contain uppercase")
            .Matches("[a-z]").WithMessage("Must contain lowercase")
            .Matches("[0-9]").WithMessage("Must contain a number");
        }
    }
}