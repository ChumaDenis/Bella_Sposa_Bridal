namespace BellaSposaBridal.Application.DTOs.Collection;

public class CreateCollectionDto
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Season { get; set; }
    public int Year { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? CoverImageUrlMobile { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public int FeaturedOrder { get; set; } = 0;
}
