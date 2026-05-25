namespace BellaSposaBridal.Application.Interfaces.Services;

public interface IStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType);
    Task DeleteAsync(string publicUrl);
}
