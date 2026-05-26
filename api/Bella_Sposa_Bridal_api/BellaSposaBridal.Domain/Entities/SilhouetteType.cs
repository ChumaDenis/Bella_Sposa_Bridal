namespace BellaSposaBridal.Domain.Entities;

public class SilhouetteType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public ICollection<Dress> Dresses { get; set; } = new List<Dress>();
}
