using BellaSposaBridal.Application.DTOs.Collection;

namespace BellaSposaBridal.Application.Interfaces.Services;

public interface ICollectionService
{
    Task<IEnumerable<CollectionDto>> GetAllAsync(bool includeDeleted = false);
    Task<IEnumerable<CollectionNameDto>> GetNamesAsync();
    Task<CollectionDto?> GetByIdAsync(Guid id);
    Task<CollectionDto?> GetBySlugAsync(string slug);
    Task<CollectionDto> CreateAsync(CreateCollectionDto dto);
    Task<CollectionDto?> UpdateAsync(Guid id, CreateCollectionDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> RestoreAsync(Guid id);
    Task ToggleActiveAsync(Guid id, bool isActive);
    Task<IEnumerable<CollectionDto>> GetFeaturedAsync();
    Task SetFeaturedAsync(Guid id, bool isFeatured, int order);
}
