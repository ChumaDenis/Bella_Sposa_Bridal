namespace BellaSposaBridal.Application.DTOs.Appointment;

public class RescheduleAppointmentDto
{
    public DateTime AppointmentDateTime { get; set; }
    public bool Force { get; set; } = false;
}
