using BellaSposaBridal.Domain.Enums;

namespace BellaSposaBridal.Application.DTOs.Appointment;

public class CreateAppointmentDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public AppointmentType Type { get; set; }
    public string? Notes { get; set; }
    public IEnumerable<Guid> ViewedDressIds { get; set; } = new List<Guid>();
}
