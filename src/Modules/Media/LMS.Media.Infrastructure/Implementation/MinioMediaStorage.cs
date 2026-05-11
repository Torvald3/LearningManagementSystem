using LMS.Media.Core.Configurations;
using LMS.Media.Core.Models;
using LMS.Media.Core.Services;
using Minio;
using Minio.DataModel.Args;

namespace LMS.Media.Infrastructure.Implementation;

public class MinioMediaStorage : IMediaStorage
{
    private readonly IMinioClient _minioClient;
    private readonly MediaStorageConfiguration _configuration;

    public MinioMediaStorage(
        IMinioClient minioClient,
        MediaStorageConfiguration configuration)
    {
        _minioClient = minioClient;
        _configuration = configuration;
    }

    public async Task UploadAsync(
        string objectKey,
        Stream content,
        long size,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        await EnsureBucketExistsAsync(cancellationToken);

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_configuration.Bucket)
            .WithObject(objectKey)
            .WithStreamData(content)
            .WithObjectSize(size)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
    }

    public async Task<MediaUrl> GetReadUrlAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        var expiresInSeconds = Math.Max(1, _configuration.PresignedUrlExpirationMinutes) * 60;

        var getObjectArgs = new PresignedGetObjectArgs()
            .WithBucket(_configuration.Bucket)
            .WithObject(objectKey)
            .WithExpiry(expiresInSeconds);

        var url = await _minioClient.PresignedGetObjectAsync(getObjectArgs);

        return new MediaUrl(
            url,
            DateTime.UtcNow.AddSeconds(expiresInSeconds));
    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        var bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(_configuration.Bucket);

        var exists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

        if (exists)
        {
            return;
        }

        var makeBucketArgs = new MakeBucketArgs()
            .WithBucket(_configuration.Bucket);

        await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
    }
}
