namespace ChatApp.Application.Interfaces;

public interface IStorageService
{
<<<<<<< HEAD
    
=======
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<bool> DeleteFileAsync(string fileUrl);
    string GetFileUrl(string fileName);
>>>>>>> a957673 (initial)
}