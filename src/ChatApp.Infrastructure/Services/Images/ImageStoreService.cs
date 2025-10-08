using ChatApp.Application.Common.Interfaces.Images;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using System.Net;

namespace ChatApp.Infrastructure.Services.Images
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = default!;
        public string ApiKey { get; set; } = default!;
        public string ApiSecret { get; set; } = default!;
        public string? Folder { get; set; }
    }

    public class ImageStoreService : IImageStoreService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _settings;

        public ImageStoreService(IOptions<CloudinarySettings> options)
        {
            _settings = options.Value;
            _cloudinary = new Cloudinary(new Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret));
        }

        public async Task<AppImageUploadResult> UploadAsync(Stream stream, string fileName, string? folder = null)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = folder ?? _settings.Folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            
            if (result.StatusCode is not HttpStatusCode.OK && result.StatusCode is not HttpStatusCode.Created)
            {
                throw new InvalidOperationException($"Cloudinary upload failed: {result.Error?.Message}");
            }

            return new AppImageUploadResult(result.SecureUrl.ToString(), result.PublicId);
        }

        public async Task<bool> DeleteAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                return false;
            }

            var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
            return result.StatusCode == HttpStatusCode.OK;
        }
    }
}
