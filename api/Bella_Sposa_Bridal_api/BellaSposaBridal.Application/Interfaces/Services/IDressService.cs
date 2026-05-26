using BellaSposaBridal.Application.DTOs;
using BellaSposaBridal.Application.DTOs.Dress;

namespace BellaSposaBridal.Application.Interfaces.Services;

public interface IDressService
{
    Task<IEnumerable<DressListDto>> GetAllActiveAsync();
    Task<IEnumerable<DressListDto>> GetAllAsync(bool includeDeleted = false);
    Task<IEnumerable<DressListDto>> GetByCollectionIdAsync(Guid collectionId);
    Task<DressDetailDto?> GetByIdAsync(Guid id);
    Task<DressDetailDto?> GetBySlugAsync(string slug);
    Task<DressDetailDto> CreateAsync(CreateDressDto dto);
    Task<DressDetailDto?> UpdateAsync(Guid id, CreateDressDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> RestoreAsync(Guid id);
    Task ToggleActiveAsync(Guid id, bool isActive);
    Task<DressPhotoDto?> AddPhotoAsync(Guid dressId, AddDressPhotoDto dto);
    Task<bool> DeletePhotoAsync(Guid dressId, Guid photoId);
    Task ReorderPhotosAsync(Guid dressId, IReadOnlyList<Guid> orderedIds);
    Task<DressVideoDto?> AddVideoAsync(Guid dressId, AddDressVideoDto dto);
    Task<bool> DeleteVideoAsync(Guid dressId, Guid videoId);
    Task<IEnumerable<NavDressItemDto>> GetNavDressesForCollectionAsync(Guid collectionId);
    Task SetNavOrderAsync(Guid collectionId, Guid dressId, int? navOrder);
    Task<PagedResult<DressListDto>> GetAllActivePagedAsync(int page, int pageSize, Guid? collectionId, int? silhouette, string? size);
    Task<PagedResult<DressListDto>> GetByCollectionIdPagedAsync(Guid collectionId, int page, int pageSize);
    Task<DressFilterMeta> GetFilterMetaAsync();
    Task<IEnumerable<DressListDto>> GetHomepageFeaturedAsync();
    Task SetHomepageFeaturedAsync(Guid id, bool isFeatured, int order);
}
