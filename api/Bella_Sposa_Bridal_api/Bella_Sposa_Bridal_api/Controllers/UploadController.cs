using BellaSposaBridal.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bella_Sposa_Bridal_api.Controllers;

[ApiController]
[Route("api/upload")]
public class UploadController : ControllerBase
{
    private readonly IStorageService _storage;

    public UploadController(IStorageService storage)
    {
        _storage = storage;
    }

    [HttpPost]
    [RequestSizeLimit(30_000_000)]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file provided" });

        var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowed.Contains(file.ContentType.ToLower()))
            return BadRequest(new { message = "Only JPEG, PNG, WebP, or GIF images are accepted" });

        await using var stream = file.OpenReadStream();
        var url = await _storage.UploadAsync(stream, file.FileName, file.ContentType);
        return Ok(new { url });
    }
}
