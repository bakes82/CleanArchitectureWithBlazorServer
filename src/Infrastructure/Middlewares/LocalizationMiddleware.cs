using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace CleanArchitecture.Blazor.Infrastructure.Middlewares;

public class LocalizationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        StringValues cultureKey = context.Request.Headers["Accept-Language"];
        if (!string.IsNullOrEmpty(cultureKey))
        {
            if (DoesCultureExist(cultureKey!))
            {
                CultureInfo culture = new CultureInfo(cultureKey!);
                Thread.CurrentThread.CurrentCulture   = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }

        await next(context);
    }

    private static bool DoesCultureExist(string cultureName)
    {
        return CultureInfo.GetCultures(CultureTypes.AllCultures)
                          .Any(culture => string.Equals(culture.Name, cultureName, StringComparison.CurrentCultureIgnoreCase));
    }
}