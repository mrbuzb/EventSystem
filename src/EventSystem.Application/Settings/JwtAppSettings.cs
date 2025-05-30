namespace EventSystem.Application.Settings;

public class JwtAppSettings
{
    private string lifetime;
    private string securityKey;
    private string audience;
    private string issuer;

    public string Lifetime
    {
        get { return lifetime; }
        set { lifetime = value; }
    }
    public string SecurityKey
    {
        get { return securityKey; }
        set { securityKey = value; }
    }
    public string Audience
    {
        get { return audience; }
        set { audience = value; }
    }
    public string Issuer
    {
        get { return issuer; }
        set { issuer = value; }
    }

    public JwtAppSettings(string issuer, string audience, string securityKey, string lifetime)
    {
        Lifetime = lifetime;
        Issuer = issuer;
        SecurityKey = securityKey;
        Audience = audience;
    }
}
