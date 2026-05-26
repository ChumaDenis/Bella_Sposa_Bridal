using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Domain.Entities;
using BellaSposaBridal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BellaSposaBridal.Infrastructure.Repositories;

public class AppointmentTypeRepository : IAppointmentTypeRepository
{
    private readonly AppDbContext _context;

    public AppointmentTypeRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<AppointmentTypeConfig>> GetAllAsync()
        => await _context.AppointmentTypeConfigs.OrderBy(t => t.DisplayOrder).AsNoTracking().ToListAsync();

    public async Task<AppointmentTypeConfig?> GetByIdAsync(int id)
        => await _context.AppointmentTypeConfigs.FindAsync(id);

    public async Task<AppointmentTypeConfig> AddAsync(AppointmentTypeConfig config)
    {
        _context.AppointmentTypeConfigs.Add(config);
        await _context.SaveChangesAsync();
        return config;
    }

    public async Task UpdateAsync(AppointmentTypeConfig config)
    {
        _context.AppointmentTypeConfigs.Update(config);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var config = await _context.AppointmentTypeConfigs.FindAsync(id);
        if (config is not null)
        {
            _context.AppointmentTypeConfigs.Remove(config);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetNextIdAsync()
    {
        var max = await _context.AppointmentTypeConfigs.MaxAsync(t => (int?)t.Id) ?? -1;
        return max + 1;
    }
}
