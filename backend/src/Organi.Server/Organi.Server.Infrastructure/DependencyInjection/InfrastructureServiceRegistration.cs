using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Infrastructure.Email;
using Organi.Server.Infrastructure.Security;
using Organi.Server.Infrastructure.Services;

namespace Organi.Server.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    // Brevo can hang; the default HttpClient timeout is 100s, which would stall a checkout.
    // The email service's try/catch does not protect against that — this does.
    private static readonly TimeSpan EmailTimeout = TimeSpan.FromSeconds(5);

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<BrevoOptions>(configuration.GetSection(BrevoOptions.SectionName));
        services.Configure<AppOptions>(configuration.GetSection(AppOptions.SectionName));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        AddEmailServices(services, configuration);

        return services;
    }

    // Without an API key the app still runs end to end — emails are logged instead of sent.
    private static void AddEmailServices(IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = configuration[$"{BrevoOptions.SectionName}:{nameof(BrevoOptions.ApiKey)}"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            services.AddSingleton<IEmailService, LogOnlyEmailService>();

            return;
        }

        services.AddHttpClient<IEmailService, BrevoEmailService>(client =>
        {
            client.BaseAddress = new Uri("https://api.brevo.com/");
            client.Timeout = EmailTimeout;
            client.DefaultRequestHeaders.Add("api-key", apiKey);
            client.DefaultRequestHeaders.Add("accept", "application/json");
        });
    }
}
