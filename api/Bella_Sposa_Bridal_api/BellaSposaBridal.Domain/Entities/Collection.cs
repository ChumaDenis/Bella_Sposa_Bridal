using BellaSposaBridal.Domain.Common;

namespace BellaSposaBridal.Domain.Entities;

public class Collection : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Season { get; set; }
    public int Year { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? CoverImageUrlMobile { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public bool IsFeatured { get; set; } = false;
    public int FeaturedOrder { get; set; } = 0;

    public ICollection<DressCollection> DressCollections { get; set; } = new List<DressCollection>();
}
