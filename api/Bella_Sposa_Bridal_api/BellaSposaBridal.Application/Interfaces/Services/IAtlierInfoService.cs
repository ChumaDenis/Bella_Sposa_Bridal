using BellaSposaBridal.Application.DTOs.AtlierInfo;

namespace BellaSposaBridal.Application.Interfaces.Services;

public interface IAtlierInfoService
{
    Task<AtlierInfoDto?> GetAsync();
    Task<AtlierInfoDto> UpsertAsync(AtlierInfoDto dto);
}
