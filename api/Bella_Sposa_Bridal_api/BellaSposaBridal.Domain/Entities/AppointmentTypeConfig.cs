namespace BellaSposaBridal.Domain.Entities;

public class AppointmentTypeConfig
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public string? MainDescription { get; set; }
    public string? Detail { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
