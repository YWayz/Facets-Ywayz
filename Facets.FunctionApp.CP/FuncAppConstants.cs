namespace Facets.FunctionApp.CP;

public static class FuncAppConstants
{
    public const string AuditTableName = "AuditLogs";

    public static string GetAppRootPath()
    {
        var local_root = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot");
        var azure_root = $"{Environment.GetEnvironmentVariable("HOME")}/site/wwwroot";
        var actual_root = local_root ?? azure_root;

        return actual_root;
    }

    public static class OnePay
    {
        public const string NotifyURLRoute = "onepay/notify-payment-update";
        public const int SuccessCode = 1;
    }
}
