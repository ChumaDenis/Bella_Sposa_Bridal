using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Application.Interfaces.Services;
using BellaSposaBridal.Infrastructure.Persistence;
using BellaSposaBridal.Infrastructure.Repositories;
using BellaSposaBridal.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BellaSposaBridal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IDressRepository, DressRepository>();
        services.AddScoped<ICollectionRepository, CollectionRepository>();
        services.AddScoped<IAtlierInfoRepository, AtlierInfoRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddSingleton<IStorageService, CloudflareR2StorageService>();

        return services;
    }
}
