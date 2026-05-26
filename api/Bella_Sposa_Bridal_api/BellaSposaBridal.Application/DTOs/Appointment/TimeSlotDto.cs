namespace BellaSposaBridal.Application.DTOs.Appointment;

public class TimeSlotDto
{
    public Guid Id { get; set; }
    public string Time { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateTimeSlotsDto
{
    public List<string> Slots { get; set; } = new();  // ordered list of time strings
}
