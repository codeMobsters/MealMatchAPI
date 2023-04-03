namespace MealMatchAPI.Services;

public interface IImageStorageService
{
    Task<string> UploadFileFromEdamam(string url);
    Task<string> UploadFile(Stream fileStream, string fileName, string contentType);
    Task<bool> DeleteFile(string fileUrl);
}