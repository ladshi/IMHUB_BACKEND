using Microsoft.AspNetCore.Http;

namespace IMHub.ApplicationLayer.Common.Interfaces.Infrastruture
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);
        Task DeleteFileAsync(string filePath);
    }
}

