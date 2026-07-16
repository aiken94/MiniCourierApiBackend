using CourierBackend.Data.Requests;
using FluentValidation;

namespace CourierBackend.Data.Validators.Package
{
    public class CreatePackageRequestValidator
        : AbstractValidator<PackageRequest>
    {
        private readonly string[] _allowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        public CreatePackageRequestValidator()
        {
            RuleFor(x => x.SenderName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.SenderEmail)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100);

            RuleFor(x => x.SenderPhoneNumber)
                .NotEmpty()
                .MinimumLength(10)
                .MaximumLength(15);

            RuleFor(x => x.SenderAddress)
                .NotEmpty();

            RuleFor(x => x.SenderCountry)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.ReceiverName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.ReceiverEmail)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100);

            RuleFor(x => x.ReceiverPhoneNumber)
                .NotEmpty()
                .MinimumLength(10)
                .MaximumLength(15);

            RuleFor(x => x.ReceiverCountry)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.ReceiverAddress)
                .NotEmpty();

            RuleFor(x => x.PackageWeight)
                .NotEmpty();

            RuleFor(x => x.PackageCost)
                .NotEmpty();

            RuleFor(x => x.PackageValue)
                .NotEmpty();

            RuleFor(x => x.PackageDescription)
                .NotEmpty();

            RuleFor(x => x.EstimatedDeliveryDate)
                .NotEmpty();

            RuleFor(x => x.PackageImage)
                .NotNull()
                .WithMessage("Package image is required.")

                .Must(BeValidExtension)
                .WithMessage("Only JPG, JPEG, PNG, and WEBP images are allowed.")

                .Must(BeValidSize)
                .WithMessage("Image size cannot exceed 5MB.");
        }

        private bool BeValidExtension(IFormFile? file)
        {
            if (file == null)
                return false;

            var extension = Path
                .GetExtension(file.FileName)
                .ToLower();

            return _allowedExtensions.Contains(extension);
        }

        private bool BeValidSize(IFormFile? file)
        {
            if (file == null)
                return false;

            return file.Length <= 5 * 1024 * 1024;
        }
    }
}