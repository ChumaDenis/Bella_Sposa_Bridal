namespace BellaSposaBridal.Application.DTOs.Appointment;

public class AppointmentFileDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
