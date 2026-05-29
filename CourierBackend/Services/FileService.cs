using CourierBackend.Helpers;

namespace CourierBackend.Services
{
    public interface IFileService
    {
        Task<string> SavePackageImageAsync(IFormFile file);
    }

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SavePackageImageAsync(IFormFile file)
        {
            if (!FileValidation.IsValidImage(file, out _))
            {
                return null;
            }

            var uploadsFolder = Path.Combine(
                _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"),
                "uploads",
                "packages"
            );

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(stream);

            return $"/uploads/packages/{fileName}";
        }
    }
}