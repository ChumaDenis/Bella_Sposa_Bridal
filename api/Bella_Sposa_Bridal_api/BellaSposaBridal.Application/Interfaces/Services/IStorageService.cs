namespace BellaSposaBridal.Application.Interfaces.Services;

public interface IStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, string folder = "dress-photos");
    Task DeleteAsync(string publicUrl);
}
