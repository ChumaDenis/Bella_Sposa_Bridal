using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Domain.Entities;
using BellaSposaBridal.Domain.Enums;
using BellaSposaBridal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BellaSposaBridal.Infrastructure.Repositories;

public class DressRepository : IDressRepository
{
    private readonly AppDbContext _context;

    public DressRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Dress>> GetAllAsync()
    {
        return await _context.Dresses
            .Include(d => d.Photos)
            .Include(d => d.Collections)
                .ThenInclude(dc => dc.Collection)
            .Include(d => d.Sizes)
            .OrderBy(d => d.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Dress>> GetAllActiveAsync()
    {
        return await _context.Dresses
            .Where(d => d.IsActive)
            .Include(d => d.Photos)
            .Include(d => d.Collections)
                .ThenInclude(dc => dc.Collection)
            .Include(d => d.Sizes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Dress>> GetByCollectionIdAsync(Guid collectionId)
    {
        return await _context.Dresses
            .Where(d => d.IsActive && d.Collections.Any(dc => dc.CollectionId == collectionId))
            .Include(d => d.Photos)
            .Include(d => d.Collections)
                .ThenInclude(dc => dc.Collection)
            .Include(d => d.Sizes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Dress?> GetByIdAsync(Guid id)
    {
        return await _context.Dresses
            .Where(d => d.Id == id)
            .Include(d => d.Photos)
            .Include(d => d.Videos)
            .Include(d => d.Sizes)
            .Include(d => d.Collections)
                .ThenInclude(dc => dc.Collection)
            .Include(d => d.RelatedDresses)
                .ThenInclude(rd => rd.Related)
                    .ThenInclude(r => r.Photos)
            .FirstOrDefaultAsync();
    }

    public async Task<Dress> AddAsync(Dress dress)
    {
        _context.Dresses.Add(dress);
        await _context.SaveChangesAsync();
        return dress;
    }

    public async Task<Dress> UpdateAsync(Dress dress)
    {
        _context.Dresses.Update(dress);
        await _context.SaveChangesAsync();
        return dress;
    }

    public async Task DeleteAsync(Guid id)
    {
        var dress = await _context.Dresses.FindAsync(id);
        if (dress is not null)
        {
            _context.Dresses.Remove(dress);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<DressPhoto> AddPhotoAsync(Guid dressId, DressPhoto photo)
    {
        photo.DressId = dressId;
        _context.Set<DressPhoto>().Add(photo);
        await _context.SaveChangesAsync();
        return photo;
    }

    public async Task<bool> DeletePhotoAsync(Guid dressId, Guid photoId)
    {
        var photo = await _context.Set<DressPhoto>()
            .FirstOrDefaultAsync(p => p.Id == photoId && p.DressId == dressId);
        if (photo is null) return false;
        _context.Set<DressPhoto>().Remove(photo);
        await _context.SaveChangesAsync();
        return true;
    }
}
