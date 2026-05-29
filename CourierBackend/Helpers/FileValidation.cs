namespace CourierBackend.Helpers
{
    public static class FileValidation
    {
        private static readonly string[] AllowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public static bool IsValidImage(IFormFile file, out string error)
        {
            error = string.Empty;

            if (file == null || file.Length == 0)
            {
                error = "Image file is required.";
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                error = "Only JPG, JPEG, PNG, and WEBP images are allowed.";
                return false;
            }

            if (file.Length > MaxFileSize)
            {
                error = "Image size cannot exceed 5MB.";
                return false;
            }

            return true;
        }
    }
}