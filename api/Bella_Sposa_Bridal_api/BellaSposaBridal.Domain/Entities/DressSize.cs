namespace BellaSposaBridal.Domain.Entities;

public class DressSize
{
    public Guid Id { get; set; }
    public Guid DressId { get; set; }
    public Dress Dress { get; set; } = null!;

    public string Size { get; set; } = string.Empty;
}
