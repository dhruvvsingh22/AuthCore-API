
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersApi.Services;

namespace UsersApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IS3Service _s3Service;
    public FilesController(IS3Service s3Service)
    {
        _s3Service = s3Service;
    }

    [HttpPost("Upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if(file==null || file.Length == 0)
        {
            return BadRequest("No File Upload");
        }

        var allowedTypes = new [] {"image/jpeg","image/png","application/pdf"};
        if(!allowedTypes.Contains(file.ContentType))
        {
            return BadRequest("Invalid file type");
        }

        if(file.Length>5*1024*1024)
        {
            return BadRequest("File too large. Max 5MB");
        }

        var key = await _s3Service.UploadFileAsync(file,"upload");
        var url = _s3Service.GetPresignedUrl(key);

        return Ok(new
        {
            key,
            url,
            message = "File upload successfully",
        });
    }

    [HttpGet("url/{key}")]
    public IActionResult GetUrl(string key)
    {
        var url = _s3Service.GetPresignedUrl(key);
        return Ok(new {url});
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string key)
    {
        await _s3Service.DeleteFileAsync(key);
        return Ok(new { message = "File deleted successfully" });
    }
}