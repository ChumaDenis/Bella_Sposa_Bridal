using BellaSposaBridal.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

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
    [RequestSizeLimit(150_000_000)]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file provided" });

        // Browsers send 'application/octet-stream' (or nothing) for extensions the OS
        // has no MIME mapping for (.heic/.tif on Windows) — fall back to the extension.
        var ct = (file.ContentType ?? "").ToLowerInvariant();
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var imageExts = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".heic", ".heif", ".avif", ".tif", ".tiff" };
        if (!ct.StartsWith("image/") && !imageExts.Contains(ext))
            return BadRequest(new { message = $"Only image files are accepted (got '{file.ContentType}', '{ext}')" });

        var isTiff = ct is "image/tiff" or "image/x-tiff" or "image/tif" or "image/x-tif"
                     || file.FileName.EndsWith(".tif",  StringComparison.OrdinalIgnoreCase)
                     || file.FileName.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase);

        if (isTiff)
        {
            await using var input = file.OpenReadStream();
            using var image  = await Image.LoadAsync(input);
            using var output = new MemoryStream();
            await image.SaveAsJpegAsync(output, new JpegEncoder { Quality = 88 });
            output.Position = 0;

            var baseName = Path.GetFileNameWithoutExtension(file.FileName);
            var url = await _storage.UploadAsync(output, baseName + ".jpg", "image/jpeg");
            return Ok(new { url });
        }

        var storedCt = ct.StartsWith("image/") ? ct : ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png"            => "image/png",
            ".webp"           => "image/webp",
            ".gif"            => "image/gif",
            ".heic"           => "image/heic",
            ".heif"           => "image/heif",
            ".avif"           => "image/avif",
            _                 => "application/octet-stream",
        };

        await using var stream = file.OpenReadStream();
        var resultUrl = await _storage.UploadAsync(stream, file.FileName, storedCt);
        return Ok(new { url = resultUrl });
    }

    [HttpPost("video")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> UploadVideo(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file provided" });

        var ct = file.ContentType.ToLowerInvariant();
        if (!ct.StartsWith("video/"))
            return BadRequest(new { message = $"Only video files are accepted (got '{file.ContentType}')" });

        await using var stream = file.OpenReadStream();
        var url = await _storage.UploadAsync(stream, file.FileName, file.ContentType);
        return Ok(new { url });
    }
}
