using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Domain.Entities;
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

    public async Task<IEnumerable<Dress>> GetAllAsync(bool includeDeleted = false)
    {
        return await _context.Dresses
            .Where(d => includeDeleted || !d.IsDeleted)
            .Include(d => d.SilhouetteType)
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
            .Where(d => d.IsActive && !d.IsDeleted
                && (!d.Collections.Any() || d.Collections.Any(dc => dc.Collection.IsActive && !dc.Collection.IsDeleted)))
            .Include(d => d.SilhouetteType)
            .Include(d => d.Photos)
            .Include(d => d.Collections)
                .ThenInclude(dc => dc.Collection)
            .Include(d => d.Sizes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Dress>> GetByCollectionIdAsync(Guid collectionId)
    {
        var orderedIds = await _context.DressCollections
            .Where(dc => dc.CollectionId == collectionId)
            .Join(_context.Dresses, dc => dc.DressId, d => d.Id,
                  (dc, d) => new { dc.DressId, NavOrder = dc.NavOrder ?? int.MaxValue, d.CreatedAt })
            .OrderBy(x => x.NavOrder)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => x.DressId)
            .ToListAsync();

        var dresses = await _context.Dresses
            .Where(d => d.IsActive && orderedIds.Contains(d.Id))
            .Include(d => d.SilhouetteType)
            .Include(d => d.Photos)
            .Include(d => d.Collections).ThenInclude(dc => dc.Collection)
            .Include(d => d.Sizes)
            .AsNoTracking()
            .ToListAsync();

        var map = dresses.ToDictionary(d => d.Id);
        return orderedIds.Where(id => map.ContainsKey(id)).Select(id => map[id]);
    }

    public async Task<Dress?> GetBySlugAsync(string slug)
    {
        return await _context.Dresses
            .Where(d => d.Slug == slug && d.IsActive && !d.IsDeleted
                && (!d.Collections.Any() || d.Collections.Any(dc => dc.Collection.IsActive && !dc.Collection.IsDeleted)))
            .Include(d => d.SilhouetteType)
            .Include(d => d.Photos)
            .Include(d => d.Videos)
            .Include(d => d.Sizes)
            .Include(d => d.Collections)
                .ThenInclude(dc => dc.Collection)
            .Include(d => d.RelatedDresses)
                .ThenInclude(rd => rd.Related)
                    .ThenInclude(r => r.Photos)
            .Include(d => d.RelatedDresses)
                .ThenInclude(rd => rd.Related)
                    .ThenInclude(r => r.SilhouetteType)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<string>> GetAllSlugsAsync()
    {
        return await _context.Dresses.Select(d => d.Slug).ToListAsync();
    }

    public async Task<Dress?> GetByIdAsync(Guid id)
    {
        return await _context.Dresses
            .Where(d => d.Id == id)
            .Include(d => d.SilhouetteType)
            .Include(d => d.Photos)
            .Include(d => d.Videos)
            .Include(d => d.Sizes)
            .Include(d => d.Collections)
                .ThenInclude(dc => dc.Collection)
            .Include(d => d.RelatedDresses)
                .ThenInclude(rd => rd.Related)
                    .ThenInclude(r => r.Photos)
            .Include(d => d.RelatedDresses)
                .ThenInclude(rd => rd.Related)
                    .ThenInclude(r => r.SilhouetteType)
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
            dress.IsDeleted = true;
            dress.DeletedAt = DateTime.UtcNow;
            dress.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RestoreAsync(Guid id)
    {
        var dress = await _context.Dresses.FindAsync(id);
        if (dress is not null)
        {
            dress.IsDeleted = false;
            dress.DeletedAt = null;
            dress.UpdatedAt = DateTime.UtcNow;
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

    public async Task<DressVideo> AddVideoAsync(Guid dressId, DressVideo video)
    {
        video.DressId = dressId;
        _context.Set<DressVideo>().Add(video);
        await _context.SaveChangesAsync();
        return video;
    }

    public async Task<bool> DeleteVideoAsync(Guid dressId, Guid videoId)
    {
        var video = await _context.Set<DressVideo>()
            .FirstOrDefaultAsync(v => v.Id == videoId && v.DressId == dressId);
        if (video is null) return false;
        _context.Set<DressVideo>().Remove(video);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task ReorderPhotosAsync(Guid dressId, IReadOnlyList<Guid> orderedIds)
    {
        var photos = await _context.Set<DressPhoto>()
            .Where(p => p.DressId == dressId)
            .ToListAsync();

        for (var i = 0; i < orderedIds.Count; i++)
        {
            var photo = photos.FirstOrDefault(p => p.Id == orderedIds[i]);
            if (photo is null) continue;
            photo.Order     = i + 1;
            photo.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<DressCollection>> GetDressCollectionsAsync(Guid collectionId)
    {
        return await _context.DressCollections
            .Where(dc => dc.CollectionId == collectionId)
            .Include(dc => dc.Dress).ThenInclude(d => d.Photos)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<(IEnumerable<Dress> Items, int TotalCount)> GetAllActivePagedAsync(
        int page, int pageSize, Guid? collectionId, int? silhouette, string? size)
    {
        var query = _context.Dresses
            .Where(d => d.IsActive && !d.IsDeleted
                && (!d.Collections.Any() || d.Collections.Any(dc => dc.Collection.IsActive && !dc.Collection.IsDeleted)))
            .AsQueryable();

        if (collectionId.HasValue)
            query = query.Where(d => d.Collections.Any(dc => dc.CollectionId == collectionId.Value));
        if (silhouette.HasValue)
            query = query.Where(d => d.SilhouetteId == silhouette.Value);
        if (!string.IsNullOrEmpty(size))
            query = query.Where(d => d.Sizes.Any(s => s.Size == size));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(d => d.SilhouetteType)
            .Include(d => d.Photos)
            .Include(d => d.Collections).ThenInclude(dc => dc.Collection)
            .Include(d => d.Sizes)
            .AsNoTracking()
            .ToListAsync();

        return (items, total);
    }

    public async Task<(IEnumerable<Dress> Items, int TotalCount)> GetByCollectionIdPagedAsync(
        Guid collectionId, int page, int pageSize)
    {
        var baseQuery = _context.DressCollections
            .Where(dc => dc.CollectionId == collectionId)
            .Join(_context.Dresses, dc => dc.DressId, d => d.Id,
                  (dc, d) => new { dc.DressId, NavOrder = dc.NavOrder ?? int.MaxValue, d.CreatedAt, d.IsActive, d.IsDeleted })
            .Where(x => x.IsActive && !x.IsDeleted);

        var total = await baseQuery.CountAsync();
        var pageIds = await baseQuery
            .OrderBy(x => x.NavOrder)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.DressId)
            .ToListAsync();

        var dresses = await _context.Dresses
            .Where(d => pageIds.Contains(d.Id))
            .Include(d => d.SilhouetteType)
            .Include(d => d.Photos)
            .Include(d => d.Collections).ThenInclude(dc => dc.Collection)
            .Include(d => d.Sizes)
            .AsNoTracking()
            .ToListAsync();

        var map = dresses.ToDictionary(d => d.Id);
        return (pageIds.Where(id => map.ContainsKey(id)).Select(id => map[id]), total);
    }

    public async Task<(IEnumerable<int> Silhouettes, IEnumerable<string> Sizes)> GetFilterMetaAsync()
    {
        var silhouettes = await _context.Dresses
            .Where(d => d.IsActive && !d.IsDeleted
                && (!d.Collections.Any() || d.Collections.Any(dc => dc.Collection.IsActive && !dc.Collection.IsDeleted)))
            .Select(d => d.SilhouetteId)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        var sizes = await _context.Dresses
            .Where(d => d.IsActive && !d.IsDeleted
                && (!d.Collections.Any() || d.Collections.Any(dc => dc.Collection.IsActive && !dc.Collection.IsDeleted)))
            .SelectMany(d => d.Sizes)
            .Select(s => s.Size)
            .Distinct()
            .ToListAsync();

        return (silhouettes, sizes);
    }

    public async Task SetNavOrderExclusiveAsync(Guid collectionId, Guid dressId, int? navOrder)
    {
        if (navOrder.HasValue)
        {
            var conflicts = await _context.DressCollections
                .Where(dc => dc.CollectionId == collectionId && dc.NavOrder == navOrder.Value && dc.DressId != dressId)
                .ToListAsync();
            foreach (var c in conflicts) c.NavOrder = null;
        }

        var target = await _context.DressCollections
            .FirstOrDefaultAsync(dc => dc.CollectionId == collectionId && dc.DressId == dressId);
        if (target is not null)
            target.NavOrder = navOrder;

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Dress>> GetHomepageFeaturedAsync()
    {
        return await _context.Dresses
            .Where(d => d.IsHomepageFeatured && d.IsActive && !d.IsDeleted
                && (!d.Collections.Any() || d.Collections.Any(dc => dc.Collection.IsActive && !dc.Collection.IsDeleted)))
            .Include(d => d.SilhouetteType)
            .Include(d => d.Photos)
            .Include(d => d.Collections).ThenInclude(dc => dc.Collection)
            .Include(d => d.Sizes)
            .OrderBy(d => d.HomepageFeaturedOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Dress>> GetSuggestionsAsync(IReadOnlyList<Guid> excludeIds, IReadOnlyList<Guid> collectionIds, int silhouette, int count)
    {
        var usedIds   = new List<Guid>(excludeIds);
        var pickedIds = new List<Guid>();

        // 1. same collection(s)
        if (collectionIds.Count > 0 && pickedIds.Count < count)
        {
            var ids = await _context.Dresses
                .Where(d => d.IsActive && !d.IsDeleted && !usedIds.Contains(d.Id)
                    && d.Collections.Any(dc => collectionIds.Contains(dc.CollectionId))
                    && (!d.Collections.Any() || d.Collections.Any(dc => dc.Collection.IsActive && !dc.Collection.IsDeleted)))
                .OrderBy(d => EF.Functions.Random())
                .Take(count - pickedIds.Count)
                .Select(d => d.Id)
                .ToListAsync();
            pickedIds.AddRange(ids);
            usedIds.AddRange(ids);
        }

        // 2. same silhouette
        if (pickedIds.Count < count)
        {
            var ids = await _context.Dresses
                .Where(d => d.IsActive && !d.IsDeleted && !usedIds.Contains(d.Id) && d.SilhouetteId == silhouette
                    && (!d.Collections.Any() || d.Collections.Any(dc => dc.Collection.IsActive && !dc.Collection.IsDeleted)))
                .OrderBy(d => EF.Functions.Random())
                .Take(count - pickedIds.Count)
                .Select(d => d.Id)
                .ToListAsync();
            pickedIds.AddRange(ids);
            usedIds.AddRange(ids);
        }

        // 3. random fill
        if (pickedIds.Count < count)
        {
            var ids = await _context.Dresses
                .Where(d => d.IsActive && !d.IsDeleted && !usedIds.Contains(d.Id)
                    && (!d.Collections.Any() || d.Collections.Any(dc => dc.Collection.IsActive && !dc.Collection.IsDeleted)))
                .OrderBy(d => EF.Functions.Random())
                .Take(count - pickedIds.Count)
                .Select(d => d.Id)
                .ToListAsync();
            pickedIds.AddRange(ids);
        }

        if (pickedIds.Count == 0) return [];

        return await _context.Dresses
            .Where(d => pickedIds.Contains(d.Id))
            .Include(d => d.SilhouetteType)
            .Include(d => d.Photos)
            .Include(d => d.Collections).ThenInclude(dc => dc.Collection)
            .Include(d => d.Sizes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task SetHomepageFeaturedAsync(Guid id, bool isFeatured, int order)
    {
        if (isFeatured)
        {
            var conflicts = await _context.Dresses
                .Where(d => d.IsHomepageFeatured && d.HomepageFeaturedOrder == order && d.Id != id)
                .ToListAsync();
            foreach (var c in conflicts)
            {
                c.IsHomepageFeatured = false;
                c.HomepageFeaturedOrder = 0;
            }
        }

        var dress = await _context.Dresses.FindAsync(id);
        if (dress is not null)
        {
            dress.IsHomepageFeatured = isFeatured;
            dress.HomepageFeaturedOrder = isFeatured ? order : 0;
            dress.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }
}
