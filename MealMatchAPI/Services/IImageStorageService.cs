namespace MealMatchAPI.Services;

public interface IImageStorageService
{
    Task<string> UploadFile(Stream fileStream, string fileName, string contentType);
    Task<bool> DeleteFile(string fileUrl);
}