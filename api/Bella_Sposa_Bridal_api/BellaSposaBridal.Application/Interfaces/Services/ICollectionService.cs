using BellaSposaBridal.Application.DTOs.Collection;

namespace BellaSposaBridal.Application.Interfaces.Services;

public interface ICollectionService
{
    Task<IEnumerable<CollectionDto>> GetAllAsync();
    Task<CollectionDto?> GetByIdAsync(Guid id);
    Task<CollectionDto> CreateAsync(CreateCollectionDto dto);
    Task<CollectionDto?> UpdateAsync(Guid id, CreateCollectionDto dto);
    Task<bool> DeleteAsync(Guid id);
}
