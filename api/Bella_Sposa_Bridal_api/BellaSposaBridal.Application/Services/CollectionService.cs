using BellaSposaBridal.Application.DTOs.Collection;
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

    public async Task<IEnumerable<CollectionDto>> GetAllAsync()
    {
        var collections = await _collectionRepository.GetAllAsync();
        return collections.Select(MapToDto);
    }

    public async Task<CollectionDto?> GetByIdAsync(Guid id)
    {
        var collection = await _collectionRepository.GetByIdAsync(id);
        return collection is null ? null : MapToDto(collection);
    }

    public async Task<CollectionDto> CreateAsync(CreateCollectionDto dto)
    {
        var collection = new Collection
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Season = dto.Season,
            Year = dto.Year,
            Description = dto.Description,
            CoverImageUrl = dto.CoverImageUrl,
            IsActive = dto.IsActive,
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

        collection.Name = dto.Name;
        collection.Season = dto.Season;
        collection.Year = dto.Year;
        collection.Description = dto.Description;
        collection.CoverImageUrl = dto.CoverImageUrl;
        collection.IsActive = dto.IsActive;
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

    private static CollectionDto MapToDto(Collection c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Season = c.Season,
        Year = c.Year,
        Description = c.Description,
        CoverImageUrl = c.CoverImageUrl,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
