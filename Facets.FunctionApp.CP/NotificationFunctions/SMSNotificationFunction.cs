using Facets.FunctionApp.CP.Exceptions;
using Facets.FunctionApp.CP.NotificationFunctions.Models;
using Facets.SharedKernal;
using Facets.SharedKernal.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Facets.FunctionApp.CP.NotificationFunctions;

public sealed class SMSNotificationFunction
{

    private readonly ILogger _logger;
    private readonly IHttpClientFactory _clientFactory;

    public SMSNotificationFunction(ILoggerFactory loggerFactory, IHttpClientFactory clientFactory)
    {
        _logger = loggerFactory.CreateLogger<SMSNotificationFunction>();
        _clientFactory = clientFactory;
    }

    [Function(nameof(SendSMSFunc))]
    public async Task SendSMSFunc([QueueTrigger(AppConstants.QueueStorage.QueueName.SMSQueue)] SMSMessage smsMessage)
    {
        try
        {
            bool isSriLankanNumber = MobileNumberHelper.IsSriLankaNumber(smsMessage.MobileNumber);

            _logger.LogInformation($"Sending SMS to: {MaskNumber(smsMessage.MobileNumber)} | Sri Lanka SMS: {isSriLankanNumber} | Subject: {smsMessage.Subject}");

            if (isSriLankanNumber)
            {
                _logger.LogInformation($"SMS Provider: TextIt | To: {MaskNumber(smsMessage.MobileNumber)}");
                await SendSMSToSriLankaNumber(smsMessage);
            }

            else
            {
                _logger.LogInformation($"SMS Provider: Twilio | To: {MaskNumber(smsMessage.MobileNumber)}");
                await SendSMSToForeignNumber(smsMessage);
            }

            _logger.LogInformation($"Sent SMS to: {MaskNumber(smsMessage.MobileNumber)} | Subject: {smsMessage.Subject}");
        }
        catch (Exception ex)
        {
            string message = $"""
                Failed sending SMS to: {MaskNumber(smsMessage.MobileNumber)} |
                Error Msg: {ex.InnerException?.Message ?? ex.Message}
                """;
            _logger.LogError(message);

            throw;
        }
    }

    private async Task SendSMSToSriLankaNumber(SMSMessage smsMessage)
    {
        var client = _clientFactory.CreateClient();

        var id = Environment.GetEnvironmentVariable("TextIt_Id");
        var password = Environment.GetEnvironmentVariable("TextIt_Password");
        var from = Environment.GetEnvironmentVariable("TextIt_From");

        string transformedToMobileNumber = RemoveSpecialCharactersFromMobileNumber(smsMessage.MobileNumber);

        // Credentials and the message travel in the query string, so this must not go over plain HTTP.
        // TextIt_BaseUrl lets you fall back to http:// only if the provider really has no TLS endpoint.
        string baseUrl = Environment.GetEnvironmentVariable("TextIt_BaseUrl") is { Length: > 0 } configured ? configured : "https://textit.biz/sendmsg/index.php";
        string url = $"{baseUrl}?id={Uri.EscapeDataString(id ?? string.Empty)}&pw={Uri.EscapeDataString(password ?? string.Empty)}&to={Uri.EscapeDataString(transformedToMobileNumber)}&text={Uri.EscapeDataString(smsMessage.Message ?? string.Empty)}";

        var smsGateWayResponse = await client.GetAsync(url);

        var smsDeliveryResponse = await smsGateWayResponse.Content.ReadAsStringAsync();

        if (smsGateWayResponse.IsSuccessStatusCode)
        {
            if (smsDeliveryResponse.StartsWith(AppConstants.TextIt.Success, StringComparison.OrdinalIgnoreCase)) return;

            else
            {
                throw new TextItExceptions(smsDeliveryResponse);
            }
        }

        else
        {
            throw new TextItExceptions(smsDeliveryResponse);
        }

        string RemoveSpecialCharactersFromMobileNumber(string mobileNumber)
        {
            return mobileNumber.StartsWith("+") ? mobileNumber.Replace("+", "00") : mobileNumber;
        }
    }

    private async Task SendSMSToForeignNumber(SMSMessage smsMessage)
    {
        string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID")!;
        string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN")!;
        string fromPhoneNumber = Environment.GetEnvironmentVariable("TWILIO_FromPhoneNumber")!;

        TwilioClient.Init(accountSid, authToken);

        await MessageResource.CreateAsync(body: smsMessage.Message,
                                          from: new PhoneNumber(fromPhoneNumber),
                                          to: new PhoneNumber(smsMessage.MobileNumber));
    }

    // Keep full phone numbers out of Application Insights.
    private static string MaskNumber(string? number)
    {
        if (string.IsNullOrWhiteSpace(number)) return "(none)";
        return number.Length <= 3 ? "***" : new string('*', number.Length - 3) + number[^3..];
    }
}
