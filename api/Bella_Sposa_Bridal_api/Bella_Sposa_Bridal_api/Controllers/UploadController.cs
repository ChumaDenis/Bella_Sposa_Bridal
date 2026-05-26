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

        var ct = file.ContentType.ToLowerInvariant();
        if (!ct.StartsWith("image/"))
            return BadRequest(new { message = $"Only image files are accepted (got '{file.ContentType}')" });

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

        await using var stream = file.OpenReadStream();
        var resultUrl = await _storage.UploadAsync(stream, file.FileName, file.ContentType);
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
