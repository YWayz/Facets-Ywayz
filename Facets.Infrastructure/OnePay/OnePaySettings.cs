namespace Facets.Infrastructure.OnePay;

public sealed class OnePaySettings
{
    public string AppID { get; set; } = null!;
    public string MerchantSecret { get; set; } = null!;
    public string AppToken { get; set; } = null!;
    public string HashSalt { get; init; } = null!;
    public string BaseURL { get; set; } = null!;
    public string PayentRequestEndPoint { get; set; } = null!;
    public string TransactionRedirectUrl { get; set; } = null!;
}
