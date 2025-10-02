namespace Core.Services.Image
{
    public record ImageUploadResult(string Url, string PublicId);

    public interface IImageStoreService
    {
        Task<ImageUploadResult> UploadAsync(Stream stream, string fileName, string? folder = null);
        Task<bool> DeleteAsync(string publicId);
    }
}
