using BellaSposaBridal.Domain.Common;

namespace BellaSposaBridal.Domain.Entities;

public class AppointmentFile : BaseEntity
{
    public Guid AppointmentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public Appointment Appointment { get; set; } = null!;
}
