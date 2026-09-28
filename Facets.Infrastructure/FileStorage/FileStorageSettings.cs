namespace Facets.Infrastructure.FileStorage;

/// <summary>
/// Bound from the "FileStorage" configuration section. All values have safe defaults.
/// </summary>
public sealed class FileStorageSettings
{
    /// <summary>
    /// When true (default), containers the app writes to are private and every file URL returned
    /// by the API is a short-lived signed link. Uploaded files include NIC scans and photos; with
    /// public containers anyone holding a link could open them without logging in, indefinitely.
    /// Set FileStorage__PrivateContainers=false to restore the old behaviour if something breaks.
    /// </summary>
    public bool PrivateContainers { get; set; } = true;

    /// <summary>How long a signed link stays valid. Long enough for a working day in the admin app.</summary>
    public int SignedUrlLifetimeHours { get; set; } = 12;

    /// <summary>
    /// Containers that must stay publicly readable and are never signed, such as the logo used in emails.
    /// Comma-separated when set through an environment variable.
    /// </summary>
    public string PublicContainers { get; set; } = "common-images";

    public IReadOnlySet<string> PublicContainerSet =>
        new HashSet<string>((PublicContainers ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                            StringComparer.OrdinalIgnoreCase);
}
