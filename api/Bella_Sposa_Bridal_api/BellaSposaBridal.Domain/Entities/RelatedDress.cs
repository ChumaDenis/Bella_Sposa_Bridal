namespace BellaSposaBridal.Domain.Entities;

public class RelatedDress
{
    public Guid DressId { get; set; }
    public Dress Dress { get; set; } = null!;

    public Guid RelatedDressId { get; set; }
    public Dress Related { get; set; } = null!;
}
