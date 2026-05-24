using BellaSposaBridal.Domain.Common;
using BellaSposaBridal.Domain.Enums;

namespace BellaSposaBridal.Domain.Entities;

public class DressPhoto : BaseEntity
{
    public Guid DressId { get; set; }
    public Dress Dress { get; set; } = null!;

    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public PhotoType Type { get; set; }
    public int Order { get; set; }
}
