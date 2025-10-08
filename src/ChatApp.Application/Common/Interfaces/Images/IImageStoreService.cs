namespace ChatApp.Application.Common.Interfaces.Images
{
    public record AppImageUploadResult(string Url, string PublicId);

    public interface IImageStoreService
    {
        Task<AppImageUploadResult> UploadAsync(Stream stream, string fileName, string? folder = null);
        Task<bool> DeleteAsync(string publicId);
    }
}
