using BellaSposaBridal.Application.DTOs.Dress;
using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Application.Interfaces.Services;
using BellaSposaBridal.Domain.Entities;
using BellaSposaBridal.Domain.Enums;

namespace BellaSposaBridal.Application.Services;

public class DressService : IDressService
{
    private readonly IDressRepository _dressRepository;

    public DressService(IDressRepository dressRepository)
    {
        _dressRepository = dressRepository;
    }

    public async Task<IEnumerable<DressListDto>> GetAllActiveAsync()
    {
        var dresses = await _dressRepository.GetAllActiveAsync();
        return dresses.Select(MapToListDto);
    }

    public async Task<IEnumerable<DressListDto>> GetByCollectionIdAsync(Guid collectionId)
    {
        var dresses = await _dressRepository.GetByCollectionIdAsync(collectionId);
        return dresses.Select(MapToListDto);
    }

    public async Task<DressDetailDto?> GetByIdAsync(Guid id)
    {
        var dress = await _dressRepository.GetByIdAsync(id);
        return dress is null ? null : MapToDetailDto(dress);
    }

    public async Task<DressDetailDto> CreateAsync(CreateDressDto dto)
    {
        var dress = new Dress
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Tagline = dto.Tagline,
            Description = dto.Description,
            Silhouette = dto.Silhouette,
            Material = dto.Material,
            CorsetType = dto.CorsetType,
            TrainDescription = dto.TrainDescription,
            Color = dto.Color,
            HasSleeves = dto.HasSleeves,
            SleeveDescription = dto.SleeveDescription,
            Decoration = dto.Decoration,
            CustomTailoringAvailable = dto.CustomTailoringAvailable,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Collections = dto.CollectionIds.Select(cId => new DressCollection { CollectionId = cId }).ToList()
        };

        var created = await _dressRepository.AddAsync(dress);
        var full = await _dressRepository.GetByIdAsync(created.Id);
        return MapToDetailDto(full!);
    }

    public async Task<DressDetailDto?> UpdateAsync(Guid id, CreateDressDto dto)
    {
        var dress = await _dressRepository.GetByIdAsync(id);
        if (dress is null) return null;

        dress.Name = dto.Name;
        dress.Tagline = dto.Tagline;
        dress.Description = dto.Description;
        dress.Silhouette = dto.Silhouette;
        dress.Material = dto.Material;
        dress.CorsetType = dto.CorsetType;
        dress.TrainDescription = dto.TrainDescription;
        dress.Color = dto.Color;
        dress.HasSleeves = dto.HasSleeves;
        dress.SleeveDescription = dto.SleeveDescription;
        dress.Decoration = dto.Decoration;
        dress.CustomTailoringAvailable = dto.CustomTailoringAvailable;
        dress.IsActive = dto.IsActive;
        dress.UpdatedAt = DateTime.UtcNow;

        dress.Collections.Clear();
        foreach (var cId in dto.CollectionIds)
            dress.Collections.Add(new DressCollection { DressId = dress.Id, CollectionId = cId });

        var updated = await _dressRepository.UpdateAsync(dress);
        return MapToDetailDto(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var dress = await _dressRepository.GetByIdAsync(id);
        if (dress is null) return false;
        await _dressRepository.DeleteAsync(id);
        return true;
    }

    private static string? GetHeroImageUrl(Dress dress)
    {
        var hero = dress.Photos.FirstOrDefault(p => p.Type == PhotoType.Hero);
        if (hero is not null) return hero.Url;
        return dress.Photos.FirstOrDefault(p => p.Type == PhotoType.Front)?.Url;
    }

    private static DressListDto MapToListDto(Dress dress) => new()
    {
        Id = dress.Id,
        Name = dress.Name,
        Tagline = dress.Tagline,
        Silhouette = dress.Silhouette,
        Color = dress.Color,
        HeroImageUrl = GetHeroImageUrl(dress),
        CollectionNames = dress.Collections
            .Select(dc => dc.Collection?.Name ?? string.Empty)
            .Where(n => !string.IsNullOrEmpty(n))
            .ToList(),
        Sizes = dress.Sizes.Select(s => s.Size).ToList()
    };

    private static DressDetailDto MapToDetailDto(Dress dress) => new()
    {
        Id = dress.Id,
        Name = dress.Name,
        Tagline = dress.Tagline,
        Description = dress.Description,
        Silhouette = dress.Silhouette,
        Material = dress.Material,
        CorsetType = dress.CorsetType,
        TrainDescription = dress.TrainDescription,
        Color = dress.Color,
        HasSleeves = dress.HasSleeves,
        SleeveDescription = dress.SleeveDescription,
        Decoration = dress.Decoration,
        CustomTailoringAvailable = dress.CustomTailoringAvailable,
        IsActive = dress.IsActive,
        CreatedAt = dress.CreatedAt,
        UpdatedAt = dress.UpdatedAt,
        Photos = dress.Photos.Select(p => new DressPhotoDto
        {
            Id = p.Id,
            Url = p.Url,
            AltText = p.AltText,
            Type = p.Type,
            Order = p.Order
        }).ToList(),
        Videos = dress.Videos.Select(v => new DressVideoDto
        {
            Id = v.Id,
            Url = v.Url,
            ThumbnailUrl = v.ThumbnailUrl,
            Type = v.Type
        }).ToList(),
        Sizes = dress.Sizes.Select(s => s.Size).ToList(),
        CollectionNames = dress.Collections
            .Select(dc => dc.Collection?.Name ?? string.Empty)
            .Where(n => !string.IsNullOrEmpty(n))
            .ToList(),
        RelatedDresses = dress.RelatedDresses.Select(rd => new DressListDto
        {
            Id = rd.Related.Id,
            Name = rd.Related.Name,
            Tagline = rd.Related.Tagline,
            Silhouette = rd.Related.Silhouette,
            Color = rd.Related.Color,
            HeroImageUrl = GetHeroImageUrl(rd.Related),
            CollectionNames = new List<string>()
        }).ToList()
    };
}
