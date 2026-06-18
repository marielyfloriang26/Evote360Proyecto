using Evote360.Application.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Evote360.Infrastructure.Services.Implementations
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public FileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var uploadsFolder = Path.Combine(_env.WebRootPath, folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/{folderName}/{uniqueFileName}";
        }

        public void DeleteFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            // fileUrl will be something like "/images/candidatos/abc.jpg"
            var path = fileUrl.TrimStart('/');
            var absolutePath = Path.Combine(_env.WebRootPath, path.Replace('/', '\\'));

            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }
        }
    }
}