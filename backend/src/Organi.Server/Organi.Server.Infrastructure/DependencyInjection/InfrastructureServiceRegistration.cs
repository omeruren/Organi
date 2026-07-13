using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Infrastructure.Security;
using Organi.Server.Infrastructure.Services;

namespace Organi.Server.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}
