using BellaSposaBridal.Domain.Enums;

namespace BellaSposaBridal.Application.DTOs.Dress;

public class DressPhotoDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public PhotoType Type { get; set; }
    public int Order { get; set; }
}
