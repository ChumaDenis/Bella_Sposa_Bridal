namespace BellaSposaBridal.Application.DTOs.Appointment;

public class DayScheduleDto
{
    public DateOnly Date { get; set; }
    public bool IsClosed { get; set; }
    public List<string>? CustomSlots { get; set; }
}

public class SetDayScheduleDto
{
    public bool IsClosed { get; set; }
    public List<string>? CustomSlots { get; set; }
}
