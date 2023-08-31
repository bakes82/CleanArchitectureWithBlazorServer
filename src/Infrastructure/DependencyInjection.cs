using CleanArchitecture.Blazor.Application.Common.Configurations;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using CleanArchitecture.Blazor.Infrastructure.Persistence.Interceptors;
using CleanArchitecture.Blazor.Infrastructure.Services.JWT;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Blazor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DashboardSettings>(configuration.GetSection(DashboardSettings.Key));
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.Key));
        services.Configure<HangfireSettings>(configuration.GetSection(HangfireSettings.Key));
        services.Configure<AuthenticationSettings>(configuration.GetSection(AuthenticationSettings.Key));

        services.Configure<AppConfigurationSettings>(configuration.GetSection(AppConfigurationSettings.Key));
        services.AddSingleton(s => s.GetRequiredService<IOptions<DashboardSettings>>()
                                    .Value);
        services.AddSingleton(s => s.GetRequiredService<IOptions<DatabaseSettings>>()
                                    .Value);
        services.AddSingleton(s => s.GetRequiredService<IOptions<AppConfigurationSettings>>()
                                    .Value);
        services.AddSingleton(s => s.GetRequiredService<IOptions<HangfireSettings>>()
                                    .Value);
        services.AddSingleton(s => s.GetRequiredService<IOptions<AuthenticationSettings>>()
                                    .Value);
        services.AddScoped<AuthenticationStateProvider, BlazorAuthStateProvider>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                                                        {
                                                            options.UseInMemoryDatabase("BlazorDashboardDb");
                                                            options.EnableSensitiveDataLogging();
                                                        });
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>((p, m) =>
                                                        {
                                                            DatabaseSettings databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>()
                                                                                                 .Value;
                                                            m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
                                                            m.UseDatabase(databaseSettings.DbProvider, databaseSettings.ConnectionString);
                                                        });
        }

        services.AddScoped<IDbContextFactory<ApplicationDbContext>, BlazorContextFactory<ApplicationDbContext>>();
        services.AddTransient<IApplicationDbContext>(provider => provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
                                                                         .CreateDbContext());
        services.AddScoped<ApplicationDbContextInitializer>();

        services.AddLocalizationServices();
        services.AddServices();

        var hangfireConfig = configuration.GetSection(HangfireSettings.Key).Get<HangfireSettings>();
        
        if (hangfireConfig?.Enabled ?? false)
        {
            services.AddHangfireService();
        }
        
        services.AddSerialization();
        services.AddMessageServices(configuration);
        services.AddSignalRServices();
        services.AddAuthenticationService(configuration);
        services.AddHttpClientService();
        services.AddControllers();
        return services;
    }
}