using CleanArchitecture.Blazor.Application.Common.Behaviours;
using CleanArchitecture.Blazor.Application.Common.PublishStrategies;
using CleanArchitecture.Blazor.Application.Common.Security;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Blazor.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(config =>
                            {
                                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                                config.NotificationPublisher = new ParallelNoWaitPublisher();
                                config.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
                                config.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
                                config.AddOpenBehavior(typeof(RequestExceptionProcessorBehavior<,>));
                                config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                                config.AddOpenBehavior(typeof(MemoryCacheBehaviour<,>));
                                config.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
                                config.AddOpenBehavior(typeof(CacheInvalidationBehaviour<,>));
                            });
        services.AddFluxor(options =>
                           {
                               options.ScanAssemblies(Assembly.GetExecutingAssembly());
                               options.UseReduxDevTools();
                           });
        services.AddLazyCache();
        services.AddScoped<RegisterFormModelFluentValidator>();
        return services;
    }
}