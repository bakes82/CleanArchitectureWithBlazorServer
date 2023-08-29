namespace CleanArchitecture.Blazor.Application.Common.Configurations;

public class HangfireSettings
{
    /// <summary>
    ///     Dashboard key constraint
    /// </summary>
    public const string Key = nameof(HangfireSettings);
    
    /// <summary>
    ///     Specifies whether to enable hangfire
    /// </summary>
    public bool Enabled { get; set; } = true;
}