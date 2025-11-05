// Infrastructure/Services/R2StorageService.cs

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Settings;

namespace EzyChat.Infrastructure.Services;


public class R2StorageService(R2Settings r2Settings) : IStorageService
{
    private readonly IAmazonS3 _s3Client = new AmazonS3Client(new BasicAWSCredentials(r2Settings.AccessKey, r2Settings.SecretKey), new AmazonS3Config
    {
        ServiceURL = $"https://{r2Settings.AccountId}.r2.cloudflarestorage.com",
    });
    
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            var key = $"chat-files/{Guid.NewGuid()}/{fileName}";

            var request = new PutObjectRequest
            {
                BucketName = r2Settings.BucketName,
                Key = key,
                InputStream = fileStream,
                ContentType = contentType,
                DisablePayloadSigning = true,
                DisableDefaultChecksumValidation = true
            };

            var result = await _s3Client.PutObjectAsync(request);

            // Check if upload was successful (HTTP 200 status code)
            if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return await GeneratePresignedUrlAsync(key);
            }
            
            throw new Exception($"Upload failed with status code: {result.HttpStatusCode}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to upload file: {ex.Message}", ex);
        }
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

    private async Task<string?> GeneratePresignedUrlAsync(string key)
    {
        var presign = new GetPreSignedUrlRequest
        {
            BucketName = r2Settings.BucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.Now.AddDays(7),
        };

        return await _s3Client.GetPreSignedURLAsync(presign);
    }
}
