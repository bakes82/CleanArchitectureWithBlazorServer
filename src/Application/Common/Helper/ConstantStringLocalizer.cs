using System.Resources;

namespace CleanArchitecture.Blazor.Application.Common.Helper;

public static class ConstantStringLocalizer
{
    public const            string          Constantstringresourceid = "CleanArchitecture.Blazor.Application.Resources.Constants.ConstantString";
    private static readonly ResourceManager Rm;

    static ConstantStringLocalizer()
    {
        Rm = new ResourceManager(Constantstringresourceid, typeof(ConstantStringLocalizer).Assembly);
    }

    public static string Localize(string key)
    {
        return Rm.GetString(key, CultureInfo.CurrentCulture) ?? key;
    }
}