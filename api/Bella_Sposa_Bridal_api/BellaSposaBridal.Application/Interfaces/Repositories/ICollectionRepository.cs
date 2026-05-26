using BellaSposaBridal.Domain.Entities;

namespace BellaSposaBridal.Application.Interfaces.Repositories;

public interface ICollectionRepository
{
    Task<IEnumerable<Collection>> GetAllAsync(bool includeDeleted = false);
    Task<IEnumerable<(Guid Id, string Name, string Slug)>> GetNamesAsync();
    Task<Collection?> GetByIdAsync(Guid id);
    Task<Collection?> GetBySlugAsync(string slug);
    Task<IEnumerable<string>> GetAllSlugsAsync();
    Task<Collection> AddAsync(Collection collection);
    Task<Collection> UpdateAsync(Collection collection);
    Task DeleteAsync(Guid id);
    Task RestoreAsync(Guid id);
}
