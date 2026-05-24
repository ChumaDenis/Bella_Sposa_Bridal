using BellaSposaBridal.Domain.Common;
using BellaSposaBridal.Domain.Enums;

namespace BellaSposaBridal.Domain.Entities;

public class Appointment : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public AppointmentType Type { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public ICollection<AppointmentViewedDress> ViewedDresses { get; set; } = new List<AppointmentViewedDress>();
}
