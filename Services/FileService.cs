using Microsoft.AspNetCore.Http;

namespace WebApp.Services
{
    public class FileService
    {
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedExtensions =
{
            ".jpg", ".jpeg", ".png", ".webp"
        };

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException("Unsupported image format.");


            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            // returnera relativ path som kan användas i <img src="">
            return "/uploads/" + fileName;
        }
    }
}
