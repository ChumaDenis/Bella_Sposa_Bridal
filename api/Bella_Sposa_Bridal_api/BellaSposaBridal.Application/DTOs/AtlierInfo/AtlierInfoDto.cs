namespace BellaSposaBridal.Application.DTOs.AtlierInfo;

public class AtlierInfoDto
{
    public Guid Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public int FittingDurationMinutes { get; set; }
    public bool IsFittingFree { get; set; }
    public int MaxGuests { get; set; }
    public bool AppointmentRequired { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? WhatsApp { get; set; }
    public string? Telegram { get; set; }
    public string? Instagram { get; set; }
    public string? WorkingHours { get; set; }
}
