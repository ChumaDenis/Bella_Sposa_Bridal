using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Domain.Entities;
using BellaSposaBridal.Domain.Enums;
using BellaSposaBridal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BellaSposaBridal.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        return await _context.Appointments
            .Include(a => a.ViewedDresses)
                .ThenInclude(vd => vd.Dress)
                    .ThenInclude(d => d != null ? d.Photos : null!)
            .Include(a => a.Files)
            .OrderBy(a => a.AppointmentDateTime)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        return await _context.Appointments
            .Where(a => a.Id == id)
            .Include(a => a.ViewedDresses)
                .ThenInclude(vd => vd.Dress)
                    .ThenInclude(d => d != null ? d.Photos : null!)
            .Include(a => a.Files)
            .FirstOrDefaultAsync();
    }

    public async Task<Appointment> AddAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment is not null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<string>> GetBookedSlotsAsync(DateOnly date)
    {
        var start = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddDays(1);

        var times = await _context.Appointments
            .Where(a => a.AppointmentDateTime >= start
                     && a.AppointmentDateTime < end
                     && a.Status != AppointmentStatus.Cancelled
                     && a.Status != AppointmentStatus.Completed)
            .Select(a => a.AppointmentDateTime)
            .ToListAsync();

        return times.Select(dt => dt.ToUniversalTime().ToString("HH:mm")).ToList();
    }

    public async Task<bool> IsSlotTakenAsync(DateTime appointmentDateTime)
    {
        return await _context.Appointments.AnyAsync(a =>
            a.AppointmentDateTime == appointmentDateTime &&
            a.Status != AppointmentStatus.Cancelled &&
            a.Status != AppointmentStatus.Completed);
    }

    public async Task RescheduleAsync(Guid id, DateTime newDateTime)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment is not null)
        {
            appointment.AppointmentDateTime = newDateTime;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateAdminNotesAsync(Guid id, string? adminNotes)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment is not null)
        {
            appointment.AdminNotes = adminNotes;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<AppointmentFile> AddFileAsync(AppointmentFile file)
    {
        _context.AppointmentFiles.Add(file);
        await _context.SaveChangesAsync();
        return file;
    }

    public async Task<AppointmentFile?> GetFileAsync(Guid fileId)
    {
        return await _context.AppointmentFiles.FindAsync(fileId);
    }

    public async Task DeleteFileAsync(Guid fileId)
    {
        var file = await _context.AppointmentFiles.FindAsync(fileId);
        if (file is not null)
        {
            _context.AppointmentFiles.Remove(file);
            await _context.SaveChangesAsync();
        }
    }
}
