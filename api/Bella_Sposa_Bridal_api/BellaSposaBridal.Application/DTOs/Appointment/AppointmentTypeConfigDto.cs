namespace BellaSposaBridal.Application.DTOs.Appointment;

public class AppointmentTypeConfigDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public string? MainDescription { get; set; }
    public string? Detail { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CreateAppointmentTypeDto
{
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public string? MainDescription { get; set; }
    public string? Detail { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateAppointmentTypeDto
{
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public string? MainDescription { get; set; }
    public string? Detail { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
