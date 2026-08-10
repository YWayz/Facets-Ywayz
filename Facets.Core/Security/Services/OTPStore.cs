using Facets.Core.Common;
using Facets.Core.Common.Dtos;
using Facets.Core.Common.Interfaces;
using Facets.Core.Security.Dtos;
using Facets.Core.Security.Interfaces;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Helpers;
using Facets.SharedKernal.Responses;
using System.Text.RegularExpressions;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Security.Services;

internal sealed class OTPStore : IOTPStore
{
    private readonly IOTPService _otpService;
    private readonly ISMSService _smsService;
    private readonly IUnitOfWork _uow;
    private readonly IVisitorService _visitorService;
    private readonly IVisitorStore _visitorStore;
    private readonly ITokenBuilder _tokenBuilder;
    private readonly IEmailService _emailService;

    private static readonly Regex EmaiMasklRegex = new Regex(@"(?<=[\w]{1})[\w\-._\+%]*(?=[\w]{1}@)", RegexOptions.Compiled);


    public OTPStore(IOTPService otpService, ISMSService smsService, IUnitOfWork uow, IVisitorService visitorService, IVisitorStore visitorStore, ITokenBuilder tokenBuilder, IEmailService emailService)
    {
        _otpService = otpService;
        _smsService = smsService;
        _uow = uow;
        _visitorService = visitorService;
        _visitorStore = visitorStore;
        _tokenBuilder = tokenBuilder;
        _emailService = emailService;
    }

    public async Task<ResponseResult<string>> GenerateOTP(GenerateOTPDto model, CancellationToken cancellationToken)
    {
        var visitorResponse = await _visitorStore.SearchVisitor(model.IdentityNumber, cancellationToken);

        if (visitorResponse.Success is false) return new(new OperationFailedException("Visitor", "Failed to search visitor"));

        var visitor = visitorResponse.Data!;

        // Return success even though visitor is not really found in the system
        if (visitor is { IsAssocifyMember: false, IsRegisteredToFacets: false }) return new(string.Empty);

        if (model.SendOTPByEmail && string.IsNullOrWhiteSpace(visitor.Email))
            return new(new OperationFailedException("Email OTP", "No email address found to send OTP"));

        bool isSriLankanNumber = MobileNumberHelper.IsSriLankaNumber(visitor.MobileNumber!);

        string sendTo = GetSendTo(model, visitor, isSriLankanNumber);

        var otpResponse = await _otpService.GenerateOTP(model.Type, model.IdentityNumber, sendTo);

        if (otpResponse.Success is false) return new(new OperationFailedException("OTP", "Failed to generate OTP"));

        await _uow.SaveChangesAsync(cancellationToken);

        var notificationSentResponse = await SendOTPNotification();

        if (notificationSentResponse.Success is false) return new(notificationSentResponse.Errors);

        var maskedSendTo = MaskSendTo(model.SendOTPByEmail, sendTo);

        return new(maskedSendTo);

        async Task<ResponseResult> SendOTPNotification()
        {
            if (model.SendOTPByEmail is true || isSriLankanNumber is false)
            {
                string template = await _emailService.GetEmailTemplate("OTPEmailTemplate.html");

                await _emailService.SendEmailByQueue(EmailBuilder.BuildOTP(sendTo,
                                                                           otpResponse.Data!.Code,
                                                                           template));

                return new();
            }

            else
            {
                await _smsService.SendSMSByQueue(SMSMessage.BuildOTPMessage(sendTo, otpResponse.Data!.Code));
                return new();
            }
        }

        string MaskSendTo(bool sendOTPByEmail, string sendTo)
        {
            string formattedSendTo = sendTo;

            if (sendTo.Length <= 2) return formattedSendTo;

            /*if (sendOTPByEmail is true)
            {
                formattedSendTo = EmaiMasklRegex.Replace(sendTo, m => new string('x', m.Length)).ToLower();
            }

            else
            {
                string last2Digits = sendTo[^2..];

                formattedSendTo = last2Digits.PadLeft(sendTo.Length - 2, 'X');
            }*/

            return formattedSendTo;
        }

        string GetSendTo(GenerateOTPDto model, Visitors.DTOs.VisitorSearchDto visitor, bool isSriLankanNumber)
        {
            string sendTo = model.SendOTPByEmail ? visitor.Email! : visitor.MobileNumber!;

            if (isSriLankanNumber is false)
            {
                sendTo = visitor.Email!;
            }

            return sendTo;
        }
    }

    public async Task<ResponseResult<PublicUserAuthenticatedDto>> VerifyOTP(VerifyOTPDto model, CancellationToken cancellationToken)
    {
        var searchResponse = await _visitorStore.SearchVisitor(model.IdentityNumber, cancellationToken);

        if (searchResponse.Success is false) return new(searchResponse.Errors);

        if (searchResponse.Data is { IsAssocifyMember: false, IsRegisteredToFacets: false })
            return new(new OperationFailedException("OTP verification", "OTP vrification failed"));

        var otpResponse = await _otpService.VerifyOTP(model.IdentityNumber, model.Code, model.Type);

        if (otpResponse.Success is false) return new(otpResponse.Errors);

        var visitorResponse = await UpdateOnlineRegisteredVisitorOTPStatus(otpResponse.Data!.Type, searchResponse.Data!.IsRegisteredToFacets);

        if (visitorResponse.Success is false) return new(visitorResponse.Errors);

        string token = _tokenBuilder.GeneratePublicUserJwtTokenAsync(model.IdentityNumber);

        await _uow.SaveChangesAsync(cancellationToken);

        return new(new PublicUserAuthenticatedDto(AuthToken: token));

        async Task<ResponseResult> UpdateOnlineRegisteredVisitorOTPStatus(OTPType type, bool isRegisteredToFacets)
        {
            if (type is not OTPType.NewVisitorOnlineRegistration || isRegisteredToFacets is false) return new();

            var visitorOTPVerificationResponse = await _visitorService.UpdateOTPVerificationStatus(model.IdentityNumber, cancellationToken);

            if (visitorOTPVerificationResponse.Success is false) return new(visitorOTPVerificationResponse.Errors);

            return new();
        }
    }
}
