using Facets.Infrastructure.OnePay.DTOs;
using Facets.Infrastructure.OnePay.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Helpers;
using Facets.SharedKernal.Responses;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
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
        try
        {
            string serializedObj = Serializer.Serialize(newPaymentRequest);

            string hash = OnePayHelper.ComputeSHA256(_onepaySettings.HashSalt, serializedObj);

            var uri = QueryHelpers.AddQueryString(_onepaySettings.PayentRequestEndPoint, "hash", hash);

            var httpResponse = await _httpClient.PostAsJsonAsync(uri, newPaymentRequest);

            if (httpResponse.IsSuccessStatusCode)
            {
                var result = await HandleResponse<OnePaymentRequestedPaymentAPIResponse>(httpResponse);

                return result;
            }

            else
            {
                string error = await httpResponse.Content.ReadAsStringAsync();

                return new(new OperationFailedException("RequestPayment", error));
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private async Task<ResponseResult<T>> HandleResponse<T>(HttpResponseMessage? httpResponse)
    {
        if (httpResponse is null) return new(new OperationFailedException("OnePayAPIResponse", "HTTP response was null"));

        var obj = await httpResponse.Content.ReadFromJsonAsync<OnePaymentResponse<T>>();

        if (obj is { Status: not AppConstants.OnePay.ResponseCodes.SuccessCode }) return HandleError(obj);

        return new(obj!.Data);
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
