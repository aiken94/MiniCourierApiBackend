namespace CourierBackend.Data.Validators.Package
{
    using CourierBackend.Data.Requests.Auth;
    using FluentValidation;

    public class ForgotPasswordValidator
        : AbstractValidator<ForgotPasswordRequest>
    {

        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}