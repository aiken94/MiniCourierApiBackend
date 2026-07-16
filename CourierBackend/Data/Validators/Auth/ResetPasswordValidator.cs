namespace CourierBackend.Data.Validators.Package
{
    using CourierBackend.Data.Requests.Auth;
    using FluentValidation;
    using CourierBackend.Helpers;

    public class ResetPasswordValidator
        : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty();

            RuleFor(x => x.NewPassword!)
                .NotEmpty().WithMessage("Password is required.")
                .StrongPassword();

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.NewPassword).WithMessage("Confirm password must match new password.");
        }
    }
}