namespace CleanArchitecture.Blazor.Application.Common.Configurations;

public class AuthenticationSettings
{
    /// <summary>
    ///     AuthenticationSettings key constraint
    /// </summary>
    public const string Key = nameof(AuthenticationSettings);

    /// <summary>
    ///     Goggle Settings
    /// </summary>
    public GoogleAuthenticationSettings Google { get; set; } = new GoogleAuthenticationSettings();

    /// <summary>
    ///     Microsoft Settings
    /// </summary>
    public MicrosoftAuthenticationSettings Microsoft { get; set; } = new MicrosoftAuthenticationSettings();
}

public class MicrosoftAuthenticationSettings
{
    /// <summary>
    ///     Specifies whether to enable
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    ///     ClientId used for authentication
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    ///     ClientSecret used for authentication
    /// </summary>
    public string? ClientSecret { get; set; }
}

public class GoogleAuthenticationSettings
{
    /// <summary>
    ///     Specifies whether to enable
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    ///     ClientId used for authentication
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    ///     ClientSecret used for authentication
    /// </summary>
    public string? ClientSecret { get; set; }
}