using Facets.Infrastructure.OnePay.DTOs;
using Facets.Infrastructure.OnePay.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Helpers;
using Facets.SharedKernal.Responses;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Json;

namespace Facets.Infrastructure.OnePay.Services;

internal sealed class OnePayAPIService : IOnePayAPIService
{
    private readonly OnePaySettings _onepaySettings;
    private readonly HttpClient _httpClient;

    public OnePayAPIService(HttpClient httpClient,
                            IOptions<OnePaySettings> onepaySettings)
    {
        _onepaySettings = onepaySettings.Value;
        _httpClient = httpClient;
    }

    public async Task<ResponseResult<OnePaymentRequestedPaymentAPIResponse>> RequestPayment(OnePayPaymentRequestDto newPaymentRequest)
    {
        // v3: hash = SHA256(app_id + currency + amount + HashSalt), hex, sent in the body
        string hashInput = $"{newPaymentRequest.AppId}{newPaymentRequest.Currency}{newPaymentRequest.Amount.ToString("0.00", CultureInfo.InvariantCulture)}";

        newPaymentRequest.Hash = OnePayHelper.ComputeSHA256(_onepaySettings.HashSalt, hashInput);

        var httpResponse = await _httpClient.PostAsJsonAsync(_onepaySettings.PaymentRequestEndPoint, newPaymentRequest);

        if (httpResponse.IsSuccessStatusCode is false)
        {
            string error = await httpResponse.Content.ReadAsStringAsync();

            return new(new OperationFailedException("RequestPayment", error));
        }

        return await HandleResponse<OnePaymentRequestedPaymentAPIResponse>(httpResponse);
    }

    public async Task<ResponseResult<OnePayTransactionStatusDto>> GetTransactionStatus(string onePayTransactionId, CancellationToken cancellationToken)
    {
        OnePayTransactionStatusRequestDto request = new()
        {
            AppId = _onepaySettings.AppID.Trim(),
            OnePayTransactionId = onePayTransactionId,
        };

        var httpResponse = await _httpClient.PostAsJsonAsync(_onepaySettings.TransactionStatusEndPoint, request, cancellationToken);

        if (httpResponse.IsSuccessStatusCode is false)
        {
            string error = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            return new(new OperationFailedException("GetTransactionStatus", error));
        }

        var result = await HandleResponse<OnePayTransactionStatusDto>(httpResponse);

        if (result.Success && result.Data is null)
            return new(new OperationFailedException("GetTransactionStatus", "OnePay returned no transaction data"));

        return result;
    }

    private async Task<ResponseResult<T>> HandleResponse<T>(HttpResponseMessage? httpResponse)
    {
        if (httpResponse is null) return new(new OperationFailedException("OnePayAPIResponse", "HTTP response was null"));

        var obj = await httpResponse.Content.ReadFromJsonAsync<OnePaymentResponse<T>>();

        if (obj is null) return new(new OperationFailedException("OnePayAPIResponse", "OnePay returned an empty response"));

        if (obj.Status is not AppConstants.OnePay.ResponseCodes.SuccessCode) return HandleError(obj);

        return new(obj.Data!);
    }

    private static ResponseResult<T> HandleError<T>(OnePaymentResponse<T>? obj)
    {
        var validationResult = new ValidationFailure()
        {
            PropertyName = obj?.Status.ToString(),
            ErrorMessage = $"OnePay: {obj?.Message}"
        };

        return new(new[] { validationResult });
    }
}
