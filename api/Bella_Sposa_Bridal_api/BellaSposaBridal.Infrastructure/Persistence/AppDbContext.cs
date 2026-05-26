using BellaSposaBridal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BellaSposaBridal.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Dress> Dresses => Set<Dress>();
    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<DressCollection> DressCollections => Set<DressCollection>();
    public DbSet<DressPhoto> DressPhotos => Set<DressPhoto>();
    public DbSet<DressVideo> DressVideos => Set<DressVideo>();
    public DbSet<DressSize> DressSizes => Set<DressSize>();
    public DbSet<RelatedDress> RelatedDresses => Set<RelatedDress>();
    public DbSet<AtlierInfo> AtlierInfos => Set<AtlierInfo>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AppointmentViewedDress> AppointmentViewedDresses => Set<AppointmentViewedDress>();
    public DbSet<AppointmentFile> AppointmentFiles => Set<AppointmentFile>();
    public DbSet<AppointmentTypeConfig> AppointmentTypeConfigs => Set<AppointmentTypeConfig>();
    public DbSet<TimeSlotConfig> TimeSlotConfigs => Set<TimeSlotConfig>();
    public DbSet<DaySchedule> DaySchedules => Set<DaySchedule>();
    public DbSet<SilhouetteType> SilhouetteTypes => Set<SilhouetteType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
