using System.ComponentModel.DataAnnotations;

namespace BellaSposaBridal.Application.DTOs.Appointment;

public class CreateAppointmentDto
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(30, MinimumLength = 1)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress, StringLength(255)]
    public string? Email { get; set; }

    public DateTime AppointmentDateTime { get; set; }

    public int Type { get; set; }

    [StringLength(2000)]
    public string? Notes { get; set; }

    public IEnumerable<Guid> ViewedDressIds { get; set; } = new List<Guid>();
}
