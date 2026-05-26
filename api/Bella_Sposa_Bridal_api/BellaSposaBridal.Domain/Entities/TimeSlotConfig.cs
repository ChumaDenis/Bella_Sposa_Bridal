namespace BellaSposaBridal.Domain.Entities;

public class TimeSlotConfig
{
    public Guid Id { get; set; }
    public string Time { get; set; } = string.Empty;  // "10:00"
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}
