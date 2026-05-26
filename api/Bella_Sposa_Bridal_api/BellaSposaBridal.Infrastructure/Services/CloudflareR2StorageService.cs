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
        var accountId  = configuration["R2:AccountId"]  ?? throw new InvalidOperationException("R2:AccountId not configured");
        var accessKey  = configuration["R2:AccessKeyId"] ?? throw new InvalidOperationException("R2:AccessKeyId not configured");
        var secretKey  = configuration["R2:SecretAccessKey"] ?? throw new InvalidOperationException("R2:SecretAccessKey not configured");
        _bucket        = configuration["R2:BucketName"] ?? throw new InvalidOperationException("R2:BucketName not configured");
        _publicUrl     = configuration["R2:PublicUrl"]!.TrimEnd('/');

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
