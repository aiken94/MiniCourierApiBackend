namespace CourierBackend.Data.Validators
{
    using CourierBackend.Data.Requests.Auth;
    using FluentValidation;

    public class LoginValidator
        : AbstractValidator<LoginRequest>
    {

        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}