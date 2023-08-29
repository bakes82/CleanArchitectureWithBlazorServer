using System.Resources;

namespace CleanArchitecture.Blazor.Application.Common.Helper;

public static class ConstantStringLocalizer
{
    public const            string          ConstantStringResourceId = "CleanArchitecture.Blazor.Application.Resources.Constants.ConstantString";
    private static readonly ResourceManager ResourceManager;

    static ConstantStringLocalizer()
    {
        ResourceManager = new ResourceManager(ConstantStringResourceId, typeof(ConstantStringLocalizer).Assembly);
    }

    public static string Localize(string key)
    {
        return ResourceManager.GetString(key, CultureInfo.CurrentCulture) ?? key;
    }
}