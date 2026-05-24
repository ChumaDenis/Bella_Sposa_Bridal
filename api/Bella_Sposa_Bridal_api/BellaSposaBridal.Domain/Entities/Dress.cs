using BellaSposaBridal.Domain.Common;
using BellaSposaBridal.Domain.Enums;

namespace BellaSposaBridal.Domain.Entities;

public class Dress : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Silhouette Silhouette { get; set; }
    public string Material { get; set; } = string.Empty;
    public string CorsetType { get; set; } = string.Empty;
    public string? TrainDescription { get; set; }
    public string Color { get; set; } = string.Empty;
    public bool HasSleeves { get; set; }
    public string? SleeveDescription { get; set; }
    public string? Decoration { get; set; }
    public bool CustomTailoringAvailable { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<DressCollection> Collections { get; set; } = new List<DressCollection>();
    public ICollection<DressPhoto> Photos { get; set; } = new List<DressPhoto>();
    public ICollection<DressVideo> Videos { get; set; } = new List<DressVideo>();
    public ICollection<DressSize> Sizes { get; set; } = new List<DressSize>();
    public ICollection<RelatedDress> RelatedDresses { get; set; } = new List<RelatedDress>();
}
