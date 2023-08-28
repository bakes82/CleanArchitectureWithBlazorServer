namespace Blazor.Server.UI.Models.Localization;

public record LanguageCode(string Code, string DisplayName, bool IsRtl = false);

public static class LocalizationConstants
{
    public static readonly LanguageCode[] SupportedLanguages =
    {
        new LanguageCode("en-US", "English"),
        new LanguageCode("fr-FR", "French"),
        new LanguageCode("de-DE", "German"),
        new LanguageCode("ja-JP", "Japanese"),
        new LanguageCode("ca-ES", "Catalan"),
        new LanguageCode("es-ES", "Spanish"),
        new LanguageCode("ru-RU", "Russian"),
        new LanguageCode("zh-CN", "Simplified Chinese")
    };
}