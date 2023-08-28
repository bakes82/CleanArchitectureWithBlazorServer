using CleanArchitecture.Blazor.Application.Common.Behaviours;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Common.PublishStrategies;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Application.Services.MultiTenant;
using CleanArchitecture.Blazor.Application.Services.Picklist;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Example;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServicesExample(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(config =>
                            {
                                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                            });
        /*services.AddFluxor(options =>
                           {
                               options.ScanAssemblies(Assembly.GetExecutingAssembly());

                           });*/
        return services;
    }
}