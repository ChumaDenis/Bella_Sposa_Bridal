using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Domain.Entities;
using BellaSposaBridal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BellaSposaBridal.Infrastructure.Repositories;

public class SilhouetteTypeRepository : ISilhouetteTypeRepository
{
    private readonly AppDbContext _context;

    public SilhouetteTypeRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<SilhouetteType>> GetAllAsync()
        => await _context.SilhouetteTypes.OrderBy(s => s.DisplayOrder).AsNoTracking().ToListAsync();

    public async Task<SilhouetteType?> GetByIdAsync(int id)
        => await _context.SilhouetteTypes.FindAsync(id);

    public async Task<bool> IsUsedByDressAsync(int id)
        => await _context.Dresses.AnyAsync(d => d.SilhouetteId == id);

    public async Task<SilhouetteType> AddAsync(SilhouetteType silhouette)
    {
        _context.SilhouetteTypes.Add(silhouette);
        await _context.SaveChangesAsync();
        return silhouette;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.SilhouetteTypes.FindAsync(id);
        if (entity is not null)
        {
            _context.SilhouetteTypes.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<SilhouetteType?> UpdateAsync(int id, string name)
    {
        var entity = await _context.SilhouetteTypes.FindAsync(id);
        if (entity is null) return null;
        entity.Name = name;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task ReorderAsync(IEnumerable<int> ids)
    {
        var idList = ids.ToList();
        for (int i = 0; i < idList.Count; i++)
        {
            var entity = await _context.SilhouetteTypes.FindAsync(idList[i]);
            if (entity is not null)
                entity.DisplayOrder = i;
        }
        await _context.SaveChangesAsync();
    }
}
