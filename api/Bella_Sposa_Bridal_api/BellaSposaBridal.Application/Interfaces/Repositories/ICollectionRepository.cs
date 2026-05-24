using BellaSposaBridal.Domain.Entities;

namespace BellaSposaBridal.Application.Interfaces.Repositories;

public interface ICollectionRepository
{
    Task<IEnumerable<Collection>> GetAllAsync();
    Task<Collection?> GetByIdAsync(Guid id);
    Task<Collection> AddAsync(Collection collection);
    Task<Collection> UpdateAsync(Collection collection);
    Task DeleteAsync(Guid id);
}
