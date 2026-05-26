namespace BellaSposaBridal.Application.DTOs.Dress;

public class CreateDressDto
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string Tagline { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Silhouette { get; set; }
    public string Material { get; set; } = string.Empty;
    public string CorsetType { get; set; } = string.Empty;
    public string? TrainDescription { get; set; }
    public string Color { get; set; } = string.Empty;
    public bool HasSleeves { get; set; }
    public string? SleeveDescription { get; set; }
    public string? Decoration { get; set; }
    public bool CustomTailoringAvailable { get; set; }
    public bool IsActive { get; set; } = true;
    public List<Guid> CollectionIds { get; set; } = new();
}
