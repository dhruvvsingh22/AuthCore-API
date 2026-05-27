using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace UsersApi.Services;
public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    public S3Service(IConfiguration config)
    {
        _bucketName = config["AWS:BucketName"]!;
        _s3Client = new AmazonS3Client(
            config["AWS:AccessKey"],
            config["AWS:SecretKey"],
            RegionEndpoint.APSouth1
        );
    }
    public async Task<string> UploadFileAsync(IFormFile file,string folder)
    {
        var key = $"{folder}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        using var stream = file.OpenReadStream();
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType,
        };

        await _s3Client.PutObjectAsync(request);
        return key;
    }
    public async Task DeleteFileAsync(string key)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
        };
        await _s3Client.DeleteObjectAsync(request);
    }
    public string GetPresignedUrl(string key,int expiryMinutes = 15)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key  = key,
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };
        return _s3Client.GetPreSignedURL(request);
    }
}