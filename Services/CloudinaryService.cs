using AutoHub.Models.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

namespace AutoHub.Services
{
    public class CloudinaryService : IFileService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0) return string.Empty;

            var uploadResult = new ImageUploadResult();

            using (var memoryStream = new MemoryStream())
            {
                // 1. Đưa file vào bộ nhớ RAM và dùng ImageSharp để xử lý
                using (var image = await Image.LoadAsync(file.OpenReadStream()))
                {
                    // 2. Resize: Nếu bề ngang ảnh > 1920px thì ép về 1920px (Tỉ lệ tự động cân bằng)
                    if (image.Width > 1920)
                    {
                        var newHeight = (int)((1920.0 / image.Width) * image.Height);
                        image.Mutate(x => x.Resize(1920, newHeight));
                    }

                    // 3. Ép cân: Đổi sang chuẩn WebP siêu nhẹ với Quality 80% (không phân biệt được bằng mắt thường)
                    var encoder = new WebpEncoder { Quality = 80 };
                    await image.SaveAsync(memoryStream, encoder);
                }

                // Đặt lại vị trí đọc Stream về 0 trước khi gửi
                memoryStream.Position = 0;
                var newFileName = Path.GetFileNameWithoutExtension(file.FileName) + ".webp";

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(newFileName, memoryStream),
                    Folder = $"AutoHub/{folderName}"
                };

                // 4. Đẩy file siêu nhẹ lên Cloudinary
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult.SecureUrl?.ToString() ?? string.Empty;
        }
    }
}
