using BellaSposaBridal.Application.Interfaces.Repositories;
using BellaSposaBridal.Infrastructure.Persistence;
using BellaSposaBridal.Infrastructure.Repositories;
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
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("BellaSposaBridal"));

        services.AddScoped<IDressRepository, DressRepository>();
        services.AddScoped<ICollectionRepository, CollectionRepository>();
        services.AddScoped<IAtlierInfoRepository, AtlierInfoRepository>();

        return services;
    }
}
