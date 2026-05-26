using BellaSposaBridal.Domain.Entities;

namespace BellaSposaBridal.Application.Interfaces.Repositories;

public interface IDressRepository
{
    Task<IEnumerable<Dress>> GetAllActiveAsync();
    Task<IEnumerable<Dress>> GetByCollectionIdAsync(Guid collectionId);
    Task<Dress?> GetByIdAsync(Guid id);
    Task<Dress?> GetBySlugAsync(string slug);
    Task<IEnumerable<string>> GetAllSlugsAsync();
    Task<IEnumerable<Dress>> GetAllAsync(bool includeDeleted = false);
    Task<Dress> AddAsync(Dress dress);
    Task<Dress> UpdateAsync(Dress dress);
    Task DeleteAsync(Guid id);
    Task RestoreAsync(Guid id);
    Task<DressPhoto> AddPhotoAsync(Guid dressId, DressPhoto photo);
    Task<bool> DeletePhotoAsync(Guid dressId, Guid photoId);
    Task ReorderPhotosAsync(Guid dressId, IReadOnlyList<Guid> orderedIds);
    Task<DressVideo> AddVideoAsync(Guid dressId, DressVideo video);
    Task<bool> DeleteVideoAsync(Guid dressId, Guid videoId);
    Task<IEnumerable<DressCollection>> GetDressCollectionsAsync(Guid collectionId);
    Task SetNavOrderExclusiveAsync(Guid collectionId, Guid dressId, int? navOrder);
    Task<(IEnumerable<Dress> Items, int TotalCount)> GetAllActivePagedAsync(int page, int pageSize, Guid? collectionId, int? silhouette, string? size);
    Task<(IEnumerable<Dress> Items, int TotalCount)> GetByCollectionIdPagedAsync(Guid collectionId, int page, int pageSize);
    Task<(IEnumerable<int> Silhouettes, IEnumerable<string> Sizes)> GetFilterMetaAsync();
    Task<IEnumerable<Dress>> GetHomepageFeaturedAsync();
    Task SetHomepageFeaturedAsync(Guid id, bool isFeatured, int order);
    Task<IEnumerable<Dress>> GetSuggestionsAsync(IReadOnlyList<Guid> excludeIds, IReadOnlyList<Guid> collectionIds, int silhouette, int count);
}
