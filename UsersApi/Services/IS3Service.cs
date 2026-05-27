
namespace UsersApi.Services;
public interface IS3Service
{
    Task<string>UploadFileAsync(IFormFile file,string folder);
    Task DeleteFileAsync(string key);
    string GetPresignedUrl(string key,int expiryMinutes = 15);
}