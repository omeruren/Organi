using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Organi.Server.Application.Common.Behaviors;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Services;

namespace Organi.Server.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}
