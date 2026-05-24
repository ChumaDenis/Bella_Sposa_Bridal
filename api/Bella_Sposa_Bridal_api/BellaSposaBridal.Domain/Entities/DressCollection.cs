namespace BellaSposaBridal.Domain.Entities;

public class DressCollection
{
    public Guid DressId { get; set; }
    public Dress Dress { get; set; } = null!;

    public Guid CollectionId { get; set; }
    public Collection Collection { get; set; } = null!;
}
