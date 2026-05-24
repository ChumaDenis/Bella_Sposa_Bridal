using BellaSposaBridal.Domain.Entities;

namespace BellaSposaBridal.Application.Interfaces.Repositories;

public interface IAtlierInfoRepository
{
    Task<AtlierInfo?> GetAsync();
    Task<AtlierInfo> UpsertAsync(AtlierInfo atlierInfo);
}
