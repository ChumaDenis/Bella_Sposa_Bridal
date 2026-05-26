namespace BellaSposaBridal.Application.DTOs.Dress;

public class NavDressItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? HeroImageUrl { get; set; }
    public int? NavOrder { get; set; }
    public bool IsActive { get; set; }
}
