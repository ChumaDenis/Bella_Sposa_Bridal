using BellaSposaBridal.Domain.Enums;

namespace BellaSposaBridal.Application.DTOs.Dress;

public class DressVideoDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public VideoType Type { get; set; }
}
