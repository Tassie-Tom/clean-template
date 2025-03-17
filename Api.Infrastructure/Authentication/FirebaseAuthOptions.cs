namespace Api.Infrastructure.Authentication;

public class FirebaseAuthOptions
{
    public const string SectionName = "FirebaseAuth";

    public string ProjectId { get; set; } = string.Empty;

    public string CredentialJson { get; set; } = string.Empty;

    public bool EmulatorEnabled { get; set; } = false;

    public string EmulatorHost { get; set; } = "localhost";

    public int EmulatorPort { get; set; } = 9099;
}