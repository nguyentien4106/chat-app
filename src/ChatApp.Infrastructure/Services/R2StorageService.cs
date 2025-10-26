<<<<<<< HEAD
namespace ChatApp.Infrastructure.Services;

public class R2StorageService
{
    
}
=======
// Infrastructure/Services/R2StorageService.cs
using Amazon.S3;
using Amazon.S3.Model;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Settings;

namespace ChatApp.Infrastructure.Services;


public class R2StorageService(R2Settings r2Settings) : IStorageService
{
    private readonly IAmazonS3 _s3Client = new AmazonS3Client(r2Settings.AccessKey, r2Settings.SecretKey, new AmazonS3Config
    {
        ServiceURL = $"https://{r2Settings.AccountId}.r2.cloudflarestorage.com",
    });
    
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var key = $"chat-files/{Guid.NewGuid()}/{fileName}";

        var request = new PutObjectRequest
        {
            BucketName = r2Settings.BucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        };

        await _s3Client.PutObjectAsync(request);

        return $"{r2Settings.PublicUrl}/{key}";
    }

    public async Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            var key = fileUrl.Replace($"{r2Settings.PublicUrl}/", "");
            
            var request = new DeleteObjectRequest
            {
                BucketName = r2Settings.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetFileUrl(string fileName)
    {
        return $"{r2Settings.PublicUrl}/{fileName}";
    }
}
>>>>>>> a957673 (initial)
