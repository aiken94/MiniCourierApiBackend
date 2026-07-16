using FluentValidation;
using CourierBackend.Data.Requests;

namespace CourierBackend.Data.Validators.Package;

public class UpdateStatusValidator : AbstractValidator<UpdatePackageStatusRequest>
{
    public UpdateStatusValidator()
    {
        RuleFor(x => x.PackageStatus)
            .NotEmpty().WithMessage("Package status is required.")
            .IsInEnum().WithMessage("Invalid package status.");
    }
}