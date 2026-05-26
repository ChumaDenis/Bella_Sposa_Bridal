using BellaSposaBridal.Domain.Entities;

namespace BellaSposaBridal.Application.Interfaces.Repositories;

public interface ISilhouetteTypeRepository
{
    Task<IEnumerable<SilhouetteType>> GetAllAsync();
    Task<SilhouetteType?> GetByIdAsync(int id);
    Task<bool> IsUsedByDressAsync(int id);
    Task<SilhouetteType> AddAsync(SilhouetteType silhouette);
    Task DeleteAsync(int id);
    Task<SilhouetteType?> UpdateAsync(int id, string name);
    Task ReorderAsync(IEnumerable<int> ids);
}
