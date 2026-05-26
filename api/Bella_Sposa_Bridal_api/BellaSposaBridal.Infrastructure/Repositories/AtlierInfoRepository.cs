using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Domain.Entities;
using BellaSposaBridal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BellaSposaBridal.Infrastructure.Repositories;

public class AtlierInfoRepository : IAtlierInfoRepository
{
    private readonly AppDbContext _context;

    public AtlierInfoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AtlierInfo?> GetAsync()
    {
        return await _context.AtlierInfos
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<AtlierInfo> UpsertAsync(AtlierInfo atlierInfo)
    {
        var existing = await _context.AtlierInfos.FirstOrDefaultAsync();

        if (existing is null)
        {
            atlierInfo.CreatedAt = DateTime.UtcNow;
            _context.AtlierInfos.Add(atlierInfo);
        }
        else
        {
            existing.Address = atlierInfo.Address;
            existing.FittingDurationMinutes = atlierInfo.FittingDurationMinutes;
            existing.IsFittingFree = atlierInfo.IsFittingFree;
            existing.MaxGuests = atlierInfo.MaxGuests;
            existing.AppointmentRequired = atlierInfo.AppointmentRequired;
            existing.Phone = atlierInfo.Phone;
            existing.WhatsApp = atlierInfo.WhatsApp;
            existing.Telegram = atlierInfo.Telegram;
            existing.Instagram = atlierInfo.Instagram;
            existing.WorkingHours = atlierInfo.WorkingHours;
            existing.VipPrice = atlierInfo.VipPrice;
            existing.HeroVideoDesktop = atlierInfo.HeroVideoDesktop;
            existing.HeroVideoMobile = atlierInfo.HeroVideoMobile;
            existing.UpdatedAt = atlierInfo.UpdatedAt;
            atlierInfo = existing;
        }

        await _context.SaveChangesAsync();
        return atlierInfo;
    }
}
