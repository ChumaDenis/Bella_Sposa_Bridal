namespace BellaSposaBridal.Application.DTOs.Collection;

public class CollectionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Season { get; set; }
    public int Year { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
