using IMHub.ApplicationLayer.Common.Interfaces.Infrastruture;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace IMHub.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _webRootPath;

        // Note: Using IHostEnvironment - WebRootPath is not available, so we use ContentRootPath
        public LocalFileStorageService(IHostEnvironment environment)
        {
            _webRootPath = Path.Combine(environment.ContentRootPath, "wwwroot");
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0) throw new Exception("File is empty");

            // Create Path: wwwroot/uploads/templates/
            string uploadPath = Path.Combine(_webRootPath, "uploads", folderName);
            
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Generate unique filename
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative URL for Database
            return $"/uploads/{folderName}/{fileName}";
        }

        public Task DeleteFileAsync(string filePath)
        {
            string fullPath = Path.Combine(_webRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            return Task.CompletedTask;
        }
    }
}

