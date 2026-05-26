using BellaSposaBridal.Application.DTOs.Silhouette;

namespace BellaSposaBridal.Application.Interfaces.Services;

public interface ISilhouetteTypeService
{
    Task<IEnumerable<SilhouetteTypeDto>> GetAllAsync();
    Task<SilhouetteTypeDto> CreateAsync(CreateSilhouetteTypeDto dto);
    /// <summary>Throws InvalidOperationException if the silhouette is used by one or more dresses.</summary>
    Task DeleteAsync(int id);
    Task<SilhouetteTypeDto?> UpdateAsync(int id, UpdateSilhouetteTypeDto dto);
    Task ReorderAsync(ReorderSilhouetteTypesDto dto);
}
