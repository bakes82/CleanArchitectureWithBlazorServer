using System.Runtime.Versioning;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

[SupportedOSPlatform("windows")] //ToDo: Is this needed?  Dont the libraries support all OS's?
public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddScoped<ExceptionHandlingMiddleware>()
                       .AddScoped<IDateTimeService, DateTimeService>()
                       .AddScoped<IExcelService, ExcelService>()
                       .AddScoped<IUploadService, UploadService>()
                       .AddScoped<IPdfService, PdfService>();
    }
}