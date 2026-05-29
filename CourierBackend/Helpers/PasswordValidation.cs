namespace CourierBackend.Helpers;

using FluentValidation;

public static class PasswordValidation
{
    public static IRuleBuilderOptions<T, string>
        StrongPassword<T>(
            this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Must contain uppercase")
            .Matches("[a-z]").WithMessage("Must contain lowercase")
            .Matches("[0-9]").WithMessage("Must contain a number");
    }
}