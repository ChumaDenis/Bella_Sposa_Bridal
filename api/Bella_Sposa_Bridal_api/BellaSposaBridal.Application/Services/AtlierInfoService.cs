using BellaSposaBridal.Application.DTOs.AtlierInfo;
using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Application.Interfaces.Services;
using BellaSposaBridal.Domain.Entities;

namespace BellaSposaBridal.Application.Services;

public class AtlierInfoService : IAtlierInfoService
{
    private readonly IAtlierInfoRepository _atlierInfoRepository;

    public AtlierInfoService(IAtlierInfoRepository atlierInfoRepository)
    {
        _atlierInfoRepository = atlierInfoRepository;
    }

    public async Task<AtlierInfoDto?> GetAsync()
    {
        var info = await _atlierInfoRepository.GetAsync();
        return info is null ? null : MapToDto(info);
    }

    public async Task<AtlierInfoDto> UpsertAsync(AtlierInfoDto dto)
    {
        var info = new AtlierInfo
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            Address = dto.Address,
            FittingDurationMinutes = dto.FittingDurationMinutes,
            IsFittingFree = dto.IsFittingFree,
            MaxGuests = dto.MaxGuests,
            AppointmentRequired = dto.AppointmentRequired,
            Phone = dto.Phone,
            WhatsApp = dto.WhatsApp,
            Telegram = dto.Telegram,
            Instagram = dto.Instagram,
            WorkingHours = dto.WorkingHours,
            VipPrice = dto.VipPrice,
            HeroVideoDesktop = dto.HeroVideoDesktop,
            HeroVideoMobile = dto.HeroVideoMobile,
            UpdatedAt = DateTime.UtcNow
        };

        var upserted = await _atlierInfoRepository.UpsertAsync(info);
        return MapToDto(upserted);
    }

    private static AtlierInfoDto MapToDto(AtlierInfo a) => new()
    {
        Id = a.Id,
        Address = a.Address,
        FittingDurationMinutes = a.FittingDurationMinutes,
        IsFittingFree = a.IsFittingFree,
        MaxGuests = a.MaxGuests,
        AppointmentRequired = a.AppointmentRequired,
        Phone = a.Phone,
        WhatsApp = a.WhatsApp,
        Telegram = a.Telegram,
        Instagram = a.Instagram,
        WorkingHours = a.WorkingHours,
        VipPrice = a.VipPrice,
        HeroVideoDesktop = a.HeroVideoDesktop,
        HeroVideoMobile = a.HeroVideoMobile
    };
}
