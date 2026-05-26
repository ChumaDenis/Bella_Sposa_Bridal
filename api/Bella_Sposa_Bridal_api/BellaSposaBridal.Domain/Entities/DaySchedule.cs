namespace BellaSposaBridal.Domain.Entities;

public class DaySchedule
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public bool IsClosed { get; set; }
    public List<string> CustomSlots { get; set; } = new();  // null/empty = use global defaults; non-empty = custom list for this day
}
