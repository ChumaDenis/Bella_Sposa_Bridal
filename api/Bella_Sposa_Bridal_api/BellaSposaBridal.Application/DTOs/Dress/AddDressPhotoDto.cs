using BellaSposaBridal.Domain.Enums;

namespace BellaSposaBridal.Application.DTOs.Dress;

public class AddDressPhotoDto
{
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public PhotoType Type { get; set; } = PhotoType.Front;
    public int Order { get; set; }
}
