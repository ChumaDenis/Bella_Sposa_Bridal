using BellaSposaBridal.Domain.Common;
using BellaSposaBridal.Domain.Enums;

namespace BellaSposaBridal.Domain.Entities;

public class DressVideo : BaseEntity
{
    public Guid DressId { get; set; }
    public Dress Dress { get; set; } = null!;

    public string Url { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public VideoType Type { get; set; }
}
