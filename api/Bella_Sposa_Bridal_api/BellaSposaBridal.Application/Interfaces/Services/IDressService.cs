using BellaSposaBridal.Application.DTOs.Dress;

namespace BellaSposaBridal.Application.Interfaces.Services;

public interface IDressService
{
    Task<IEnumerable<DressListDto>> GetAllActiveAsync();
    Task<IEnumerable<DressListDto>> GetAllAsync();
    Task<IEnumerable<DressListDto>> GetByCollectionIdAsync(Guid collectionId);
    Task<DressDetailDto?> GetByIdAsync(Guid id);
    Task<DressDetailDto> CreateAsync(CreateDressDto dto);
    Task<DressDetailDto?> UpdateAsync(Guid id, CreateDressDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task ToggleActiveAsync(Guid id, bool isActive);
    Task<DressPhotoDto?> AddPhotoAsync(Guid dressId, AddDressPhotoDto dto);
    Task<bool> DeletePhotoAsync(Guid dressId, Guid photoId);
}
