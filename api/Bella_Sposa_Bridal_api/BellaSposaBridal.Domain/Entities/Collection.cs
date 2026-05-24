using BellaSposaBridal.Domain.Common;

namespace BellaSposaBridal.Domain.Entities;

public class Collection : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Season { get; set; }
    public int Year { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<DressCollection> DressCollections { get; set; } = new List<DressCollection>();
}
