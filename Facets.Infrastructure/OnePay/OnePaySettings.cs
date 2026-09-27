namespace Facets.Infrastructure.OnePay;

public sealed class OnePaySettings
{
    public string AppID { get; set; } = null!;
    public string MerchantSecret { get; set; } = null!;
    public string AppToken { get; set; } = null!;
    public string HashSalt { get; init; } = null!;
    public string BaseURL { get; set; } = null!;
    public string PaymentRequestEndPoint { get; set; } = null!;

    // v3 endpoint used to verify a transaction with OnePay. Relative to BaseURL, like PaymentRequestEndPoint.
    public string TransactionStatusEndPoint { get; set; } = "v3/transaction/status/";
    public string TransactionRedirectUrl { get; set; } = null!;
}
