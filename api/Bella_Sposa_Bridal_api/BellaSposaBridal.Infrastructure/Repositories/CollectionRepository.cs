using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Domain.Entities;
using BellaSposaBridal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BellaSposaBridal.Infrastructure.Repositories;

public class CollectionRepository : ICollectionRepository
{
    private readonly AppDbContext _context;

    public CollectionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Collection>> GetAllAsync(bool includeDeleted = false)
    {
        return await _context.Collections
            .Where(c => includeDeleted || !c.IsDeleted)
            .OrderByDescending(c => c.Year)
            .ThenByDescending(c => c.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<(Guid Id, string Name, string Slug)>> GetNamesAsync()
    {
        var rows = await _context.Collections
            .OrderBy(c => c.Name)
            .Select(c => new { c.Id, c.Name, c.Slug })
            .AsNoTracking()
            .ToListAsync();
        return rows.Select(x => (x.Id, x.Name, x.Slug));
    }

    public async Task<Collection?> GetByIdAsync(Guid id)
    {
        return await _context.Collections.FindAsync(id);
    }

    public async Task<Collection?> GetBySlugAsync(string slug)
    {
        return await _context.Collections
            .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive && !c.IsDeleted);
    }

    public async Task<IEnumerable<string>> GetAllSlugsAsync()
    {
        return await _context.Collections.Select(c => c.Slug).ToListAsync();
    }

    public async Task<Collection> AddAsync(Collection collection)
    {
        _context.Collections.Add(collection);
        await _context.SaveChangesAsync();
        return collection;
    }

    public async Task<Collection> UpdateAsync(Collection collection)
    {
        _context.Collections.Update(collection);
        await _context.SaveChangesAsync();
        return collection;
    }

    public async Task DeleteAsync(Guid id)
    {
        var collection = await _context.Collections.FindAsync(id);
        if (collection is not null)
        {
            collection.IsDeleted = true;
            collection.DeletedAt = DateTime.UtcNow;
            collection.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RestoreAsync(Guid id)
    {
        var collection = await _context.Collections.FindAsync(id);
        if (collection is not null)
        {
            collection.IsDeleted = false;
            collection.DeletedAt = null;
            collection.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
