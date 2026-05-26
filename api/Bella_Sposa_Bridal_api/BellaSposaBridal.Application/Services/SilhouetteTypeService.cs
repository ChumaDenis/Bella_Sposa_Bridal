using BellaSposaBridal.Application.DTOs.Silhouette;
using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Application.Interfaces.Services;
using BellaSposaBridal.Domain.Entities;

namespace BellaSposaBridal.Application.Services;

public class SilhouetteTypeService : ISilhouetteTypeService
{
    private readonly ISilhouetteTypeRepository _repo;

    public SilhouetteTypeService(ISilhouetteTypeRepository repo) => _repo = repo;

    public async Task<IEnumerable<SilhouetteTypeDto>> GetAllAsync()
    {
        var all = await _repo.GetAllAsync();
        return all.Select(s => new SilhouetteTypeDto { Id = s.Id, Name = s.Name, DisplayOrder = s.DisplayOrder });
    }

    public async Task<SilhouetteTypeDto> CreateAsync(CreateSilhouetteTypeDto dto)
    {
        var all = (await _repo.GetAllAsync()).ToList();
        var nextId    = all.Count > 0 ? all.Max(s => s.Id) + 1 : 0;
        var nextOrder = all.Count > 0 ? all.Max(s => s.DisplayOrder) + 1 : 0;

        var entity = new SilhouetteType
        {
            Id           = nextId,
            Name         = dto.Name.Trim(),
            DisplayOrder = nextOrder
        };

        var created = await _repo.AddAsync(entity);
        return new SilhouetteTypeDto { Id = created.Id, Name = created.Name, DisplayOrder = created.DisplayOrder };
    }

    public async Task DeleteAsync(int id)
    {
        if (await _repo.IsUsedByDressAsync(id))
            throw new InvalidOperationException(
                "Неможливо видалити силует, який використовується одним або більше платтями.");

        await _repo.DeleteAsync(id);
    }

    public async Task<SilhouetteTypeDto?> UpdateAsync(int id, UpdateSilhouetteTypeDto dto)
    {
        var entity = await _repo.UpdateAsync(id, dto.Name.Trim());
        if (entity is null) return null;
        return new SilhouetteTypeDto { Id = entity.Id, Name = entity.Name, DisplayOrder = entity.DisplayOrder };
    }

    public async Task ReorderAsync(ReorderSilhouetteTypesDto dto)
        => await _repo.ReorderAsync(dto.Ids);
}
