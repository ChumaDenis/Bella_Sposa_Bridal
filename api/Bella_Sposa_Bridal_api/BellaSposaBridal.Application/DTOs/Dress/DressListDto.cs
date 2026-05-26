namespace BellaSposaBridal.Application.DTOs.Dress;

public class DressListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public int Silhouette { get; set; }
    public string SilhouetteName { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? HeroImageUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsHomepageFeatured { get; set; }
    public int HomepageFeaturedOrder { get; set; }
    public List<string> CollectionNames { get; set; } = new();
    public List<string> Sizes { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
