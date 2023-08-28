using System.Runtime.Versioning;


namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

[SupportedOSPlatform("windows")]
public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddScoped<ExceptionHandlingMiddleware>()
                       .AddScoped<IDateTime, DateTimeService>()
                       .AddScoped<IExcelService, ExcelService>()
                       .AddScoped<IUploadService, UploadService>()
                       .AddScoped<IPdfService, PdfService>();
    }
}