using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoHub.Services
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(IFormFile file, string folderName);

        Task<string> UploadThumbnailAsync(IFormFile file, string folderName);

        Task<string> UploadMultipleImagesAsync(List<IFormFile> files, string folderName);
    }
}
