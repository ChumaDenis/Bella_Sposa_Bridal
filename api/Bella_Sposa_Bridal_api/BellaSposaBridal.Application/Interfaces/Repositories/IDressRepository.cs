using BellaSposaBridal.Domain.Entities;

namespace BellaSposaBridal.Application.Interfaces.Repositories;

public interface IDressRepository
{
    Task<IEnumerable<Dress>> GetAllActiveAsync();
    Task<IEnumerable<Dress>> GetByCollectionIdAsync(Guid collectionId);
    Task<Dress?> GetByIdAsync(Guid id);
    Task<IEnumerable<Dress>> GetAllAsync();
    Task<Dress> AddAsync(Dress dress);
    Task<Dress> UpdateAsync(Dress dress);
    Task DeleteAsync(Guid id);
    Task<DressPhoto> AddPhotoAsync(Guid dressId, DressPhoto photo);
    Task<bool> DeletePhotoAsync(Guid dressId, Guid photoId);
}
