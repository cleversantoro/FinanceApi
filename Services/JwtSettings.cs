namespace FinanceApi.Services;

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public int ExpirationHours { get; set; } = 2;
}
