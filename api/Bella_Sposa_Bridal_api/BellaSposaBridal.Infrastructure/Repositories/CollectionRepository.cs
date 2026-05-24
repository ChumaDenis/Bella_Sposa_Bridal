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

    public async Task<IEnumerable<Collection>> GetAllAsync()
    {
        return await _context.Collections
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Collection?> GetByIdAsync(Guid id)
    {
        return await _context.Collections.FindAsync(id);
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
            _context.Collections.Remove(collection);
            await _context.SaveChangesAsync();
        }
    }
}
