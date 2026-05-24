using BellaSposaBridal.Domain.Enums;

namespace BellaSposaBridal.Application.DTOs.Dress;

public class DressListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public Silhouette Silhouette { get; set; }
    public string Color { get; set; } = string.Empty;
    public string? HeroImageUrl { get; set; }
    public List<string> CollectionNames { get; set; } = new();
}
