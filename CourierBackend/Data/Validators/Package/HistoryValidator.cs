using CourierBackend.Data.Requests;
using FluentValidation;

namespace CourierBackend.Data.Validators.Package
{
    public class DeliveryHistoryRequestValidator
        : AbstractValidator<DeliveryHistoryRequest>
    {

        string[] allowedLocations =
        [
            "Origin",
            "Destination"
        ];


        public DeliveryHistoryRequestValidator()
        {
            RuleFor(x => x.PackageId)
                .NotEmpty();

            RuleFor(x => x.Remarks)
                .NotEmpty()
                .MaximumLength(1000);

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(100)
                .Must(location => allowedLocations.Contains(location))
                .WithMessage("Invalid location value.");

            RuleFor(x => x.Date)
                .NotEmpty();
        }
    }
}