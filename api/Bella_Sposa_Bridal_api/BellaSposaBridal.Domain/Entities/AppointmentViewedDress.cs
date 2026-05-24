namespace BellaSposaBridal.Domain.Entities;

public class AppointmentViewedDress
{
    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    public int Order { get; set; }      // 1–5
    public Guid? DressId { get; set; }  // nullable: dress may be deleted later
    public Dress? Dress { get; set; }
}
