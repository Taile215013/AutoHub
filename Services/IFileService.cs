using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AutoHub.Services
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(IFormFile file, string folderName);
    }
}
