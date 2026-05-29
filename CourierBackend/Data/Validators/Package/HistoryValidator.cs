using CourierBackend.Data.Requests;
using FluentValidation;

namespace CourierBackend.Data.Validators
{
    public class DeliveryHistoryRequestValidator
        : AbstractValidator<DeliveryHistoryRequest>
    {

        public DeliveryHistoryRequestValidator()
        {
            RuleFor(x => x.PackageId)
                .NotEmpty();

            RuleFor(x => x.Remarks)
                .NotEmpty()
                .MaximumLength(1000);

            RuleFor(x => x.Location)
                .NotEmpty()
                .IsInEnum();

            RuleFor(x => x.Date)
                .NotEmpty();
        }
    }
}