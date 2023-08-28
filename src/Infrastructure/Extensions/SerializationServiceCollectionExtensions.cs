using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using CleanArchitecture.Blazor.Infrastructure.Services.Serialization;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public static class SerializationServiceCollectionExtensions
{
    public static IServiceCollection AddSerialization(this IServiceCollection services)
    {
        return services.AddSingleton<ISerializer, SystemTextJsonSerializer>();
    }
}