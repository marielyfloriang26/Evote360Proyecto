using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Evote360.Application.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);
        void DeleteFile(string fileUrl);
    }
}