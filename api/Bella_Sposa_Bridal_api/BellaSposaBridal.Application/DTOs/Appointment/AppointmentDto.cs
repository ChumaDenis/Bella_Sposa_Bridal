using BellaSposaBridal.Application.DTOs.Dress;

namespace BellaSposaBridal.Application.DTOs.Appointment;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public IEnumerable<DressListDto?> ViewedDresses { get; set; } = new List<DressListDto?>();
    public DateTime CreatedAt { get; set; }
}
