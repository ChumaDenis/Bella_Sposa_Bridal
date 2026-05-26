using BellaSposaBridal.Application.DTOs.Collection;
using BellaSposaBridal.Application.Helpers;
using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Application.Interfaces.Services;
using BellaSposaBridal.Domain.Entities;

namespace BellaSposaBridal.Application.Services;

public class CollectionService : ICollectionService
{
    private readonly ICollectionRepository _collectionRepository;

    public CollectionService(ICollectionRepository collectionRepository)
    {
        _collectionRepository = collectionRepository;
    }

    public async Task<IEnumerable<CollectionDto>> GetAllAsync(bool includeDeleted = false)
    {
        var collections = await _collectionRepository.GetAllAsync(includeDeleted);
        return collections.Select(MapToDto);
    }

    public async Task<IEnumerable<CollectionNameDto>> GetNamesAsync()
    {
        var names = await _collectionRepository.GetNamesAsync();
        return names.Select(x => new CollectionNameDto { Id = x.Id, Name = x.Name, Slug = x.Slug });
    }

    public async Task<CollectionDto?> GetByIdAsync(Guid id)
    {
        var collection = await _collectionRepository.GetByIdAsync(id);
        return collection is null ? null : MapToDto(collection);
    }

    public async Task<CollectionDto?> GetBySlugAsync(string slug)
    {
        var collection = await _collectionRepository.GetBySlugAsync(slug);
        return collection is null ? null : MapToDto(collection);
    }

    public async Task<CollectionDto> CreateAsync(CreateCollectionDto dto)
    {
        var baseSlug = string.IsNullOrWhiteSpace(dto.Slug)
            ? SlugHelper.Generate(dto.Name)
            : SlugHelper.Generate(dto.Slug);
        var existingSlugs = await _collectionRepository.GetAllSlugsAsync();
        var slug = SlugHelper.EnsureUnique(baseSlug, existingSlugs);

        var collection = new Collection
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Slug = slug,
            Season = dto.Season,
            Year = dto.Year,
            Description = dto.Description,
            CoverImageUrl = dto.CoverImageUrl,
            CoverImageUrlMobile = dto.CoverImageUrlMobile,
            IsActive = dto.IsActive,
            IsFeatured = dto.IsFeatured,
            FeaturedOrder = dto.FeaturedOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _collectionRepository.AddAsync(collection);
        return MapToDto(created);
    }

    public async Task<CollectionDto?> UpdateAsync(Guid id, CreateCollectionDto dto)
    {
        var collection = await _collectionRepository.GetByIdAsync(id);
        if (collection is null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Slug))
        {
            var candidate = SlugHelper.Generate(dto.Slug);
            if (candidate != collection.Slug)
            {
                var others = (await _collectionRepository.GetAllSlugsAsync()).Where(s => s != collection.Slug);
                collection.Slug = SlugHelper.EnsureUnique(candidate, others);
            }
        }

        collection.Name = dto.Name;
        collection.Season = dto.Season;
        collection.Year = dto.Year;
        collection.Description = dto.Description;
        collection.CoverImageUrl = dto.CoverImageUrl;
        collection.CoverImageUrlMobile = dto.CoverImageUrlMobile;
        collection.IsActive = dto.IsActive;
        collection.IsFeatured = dto.IsFeatured;
        collection.FeaturedOrder = dto.FeaturedOrder;
        collection.UpdatedAt = DateTime.UtcNow;

        var updated = await _collectionRepository.UpdateAsync(collection);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var collection = await _collectionRepository.GetByIdAsync(id);
        if (collection is null) return false;
        await _collectionRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> RestoreAsync(Guid id)
    {
        var collection = await _collectionRepository.GetByIdAsync(id);
        if (collection is null) return false;
        await _collectionRepository.RestoreAsync(id);
        return true;
    }

    public async Task ToggleActiveAsync(Guid id, bool isActive)
    {
        var collection = await _collectionRepository.GetByIdAsync(id);
        if (collection is null) return;
        collection.IsActive = isActive;
        collection.UpdatedAt = DateTime.UtcNow;
        await _collectionRepository.UpdateAsync(collection);
    }

    public async Task<IEnumerable<CollectionDto>> GetFeaturedAsync()
    {
        var all = await _collectionRepository.GetAllAsync();
        return all
            .Where(c => c.IsFeatured && c.IsActive)
            .OrderBy(c => c.FeaturedOrder)
            .Take(2)
            .Select(MapToDto);
    }

    public async Task SetFeaturedAsync(Guid id, bool isFeatured, int order)
    {
        // Evict whatever was previously occupying this slot
        if (isFeatured)
        {
            var all = await _collectionRepository.GetAllAsync();
            var displaced = all.FirstOrDefault(c => c.Id != id && c.IsFeatured && c.FeaturedOrder == order);
            if (displaced is not null)
            {
                displaced.IsFeatured = false;
                displaced.UpdatedAt  = DateTime.UtcNow;
                await _collectionRepository.UpdateAsync(displaced);
            }
        }

        var collection = await _collectionRepository.GetByIdAsync(id);
        if (collection is null) return;
        collection.IsFeatured    = isFeatured;
        collection.FeaturedOrder = order;
        collection.UpdatedAt     = DateTime.UtcNow;
        await _collectionRepository.UpdateAsync(collection);
    }

    private static CollectionDto MapToDto(Collection c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        Season = c.Season,
        Year = c.Year,
        Description = c.Description,
        CoverImageUrl = c.CoverImageUrl,
        CoverImageUrlMobile = c.CoverImageUrlMobile,
        IsActive = c.IsActive,
        IsDeleted = c.IsDeleted,
        DeletedAt = c.DeletedAt,
        IsFeatured = c.IsFeatured,
        FeaturedOrder = c.FeaturedOrder,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
