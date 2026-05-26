namespace BellaSposaBridal.Application.DTOs.Dress;

public class DressDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Silhouette { get; set; }
    public string SilhouetteName { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public string CorsetType { get; set; } = string.Empty;
    public string? TrainDescription { get; set; }
    public string Color { get; set; } = string.Empty;
    public bool HasSleeves { get; set; }
    public string? SleeveDescription { get; set; }
    public string? Decoration { get; set; }
    public bool CustomTailoringAvailable { get; set; }
    public bool IsActive { get; set; }
    public bool IsHomepageFeatured { get; set; }
    public int HomepageFeaturedOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<DressPhotoDto> Photos { get; set; } = new();
    public List<DressVideoDto> Videos { get; set; } = new();
    public List<string> Sizes { get; set; } = new();
    public List<string> CollectionNames { get; set; } = new();
    public List<Guid> CollectionIds { get; set; } = new();
    public List<DressListDto> RelatedDresses { get; set; } = new();
}
