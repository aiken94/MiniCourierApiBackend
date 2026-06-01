using CourierBackend.Data.Requests;
using FluentValidation;

namespace CourierBackend.Data.Validators
{
    public class TrackPackageRequestValidator
        : AbstractValidator<TrackPackageRequest>
    {

        public TrackPackageRequestValidator()
        {
            RuleFor(x => x.TrackingNumber)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}