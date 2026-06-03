using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using BellaSposaBridal.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace BellaSposaBridal.Infrastructure.Services;

public class CloudflareR2StorageService : IStorageService, IDisposable
{
    private readonly AmazonS3Client _client;
    private readonly string _bucket;
    private readonly string _publicUrl;

    public CloudflareR2StorageService(IConfiguration configuration)
    {
        var accountId  = configuration["R2:AccountId"];
        var accessKey  = configuration["R2:AccessKeyId"];
        var secretKey  = configuration["R2:SecretAccessKey"];
        _bucket        = configuration["R2:BucketName"] ?? "";
        var publicUrl  = configuration["R2:PublicUrl"];

        if (string.IsNullOrWhiteSpace(accountId))  throw new InvalidOperationException("R2:AccountId is not configured or is empty. Set R2__AccountId in Railway Variables.");
        if (string.IsNullOrWhiteSpace(accessKey))  throw new InvalidOperationException("R2:AccessKeyId is not configured or is empty. Set R2__AccessKeyId in Railway Variables.");
        if (string.IsNullOrWhiteSpace(secretKey))  throw new InvalidOperationException("R2:SecretAccessKey is not configured or is empty. Set R2__SecretAccessKey in Railway Variables.");
        if (string.IsNullOrWhiteSpace(_bucket))    throw new InvalidOperationException("R2:BucketName is not configured or is empty. Set R2__BucketName in Railway Variables.");
        if (string.IsNullOrWhiteSpace(publicUrl))  throw new InvalidOperationException("R2:PublicUrl is not configured or is empty. Set R2__PublicUrl in Railway Variables.");

        _publicUrl = publicUrl.TrimEnd('/');

        var config = new AmazonS3Config
        {
            ServiceURL     = $"https://{accountId}.r2.cloudflarestorage.com",
            ForcePathStyle = true
        };
        _client = new AmazonS3Client(new BasicAWSCredentials(accessKey, secretKey), config);
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, string folder = "dress-photos")
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var key = $"{folder}/{Guid.NewGuid()}{ext}";

        await _client.PutObjectAsync(new PutObjectRequest
        {
            BucketName  = _bucket,
            Key         = key,
            InputStream = stream,
            ContentType = contentType,
            DisablePayloadSigning = true
        });

        return $"{_publicUrl}/{key}";
    }

    public async Task DeleteAsync(string publicUrl)
    {
        var key = publicUrl.Replace($"{_publicUrl}/", "");
        await _client.DeleteObjectAsync(_bucket, key);
    }

    public void Dispose() => _client.Dispose();
}
