using BellaSposaBridal.Application.Interfaces.Services;
using BellaSposaBridal.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BellaSposaBridal.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDressService, DressService>();
        services.AddScoped<ICollectionService, CollectionService>();
        services.AddScoped<IAtlierInfoService, AtlierInfoService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        return services;
    }
}
