using AutoHub.Models.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
            return await ProcessAndUploadAsync(file, folderName, maxWidth: 1920, quality: 80);
        }

        public async Task<string> UploadThumbnailAsync(IFormFile file, string folderName)
        {
            return await ProcessAndUploadAsync(file, $"thumbnails/{folderName}", maxWidth: 480, quality: 60);
        }

        public async Task<string> UploadMultipleImagesAsync(List<IFormFile> files, string folderName)
        {
            if (files == null || files.Count == 0) return string.Empty;

            var urls = new List<string>();
            foreach (var file in files)
            {
                var url = await UploadImageAsync(file, folderName);
                if (!string.IsNullOrEmpty(url))
                {
                    urls.Add(url);
                }
            }

            return urls.Count > 0 ? JsonSerializer.Serialize(urls) : string.Empty;
        }

        /// <summary>
        /// Logic xử lý ảnh dùng chung: resize + nén WebP + upload Cloudinary
        /// </summary>
        private async Task<string> ProcessAndUploadAsync(IFormFile file, string folderName, int maxWidth, int quality)
        {
            if (file == null || file.Length == 0) return string.Empty;

            var uploadResult = new ImageUploadResult();

            using (var memoryStream = new MemoryStream())
            {
                // 1. Đưa file vào bộ nhớ RAM và dùng ImageSharp để xử lý
                using (var image = await Image.LoadAsync(file.OpenReadStream()))
                {
                    // 2. Resize: Nếu bề ngang ảnh > maxWidth thì ép về maxWidth (Tỉ lệ tự động cân bằng)
                    if (image.Width > maxWidth)
                    {
                        var newHeight = (int)(((double)maxWidth / image.Width) * image.Height);
                        image.Mutate(x => x.Resize(maxWidth, newHeight));
                    }

                    // 3. Ép cân: Đổi sang chuẩn WebP siêu nhẹ
                    var encoder = new WebpEncoder { Quality = quality };
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

