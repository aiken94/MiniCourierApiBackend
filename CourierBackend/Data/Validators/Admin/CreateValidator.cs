namespace CourierBackend.Data.Validators.Admin
{
    using FluentValidation;
    using CourierBackend.Data.Requests;
    using CourierBackend.Helpers;

    public class CreateValidator : AbstractValidator<AdminRequest>
    {
        public CreateValidator()
        {
            RuleFor(request => request.Role)
                .Must(role => role == 0 || role == 1)
                .WithMessage("Invalid role type.");

            RuleFor(request => request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(request => request.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            RuleFor(request => request.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

            RuleFor(request => request.Password!)
                .NotEmpty().WithMessage("Password is required.")
                .StrongPassword();
        }
    }
}