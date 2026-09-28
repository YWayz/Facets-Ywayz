using Facets.Core.Common;
using Facets.Core.Common.Dtos;
using Facets.Core.Common.Interfaces;
using Facets.Core.Security.Interfaces;
using Facets.Core.Visitors.Events;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Helpers;
using Facets.SharedKernal.Responses;
using MediatR;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.EventHandlers;

internal sealed class OnlineVisitorCreatingSendOTPEventHandler : INotificationHandler<VisitorCreatingEvent>
{
    private readonly IOTPService _otpService;
    private readonly ISMSService _smsService;
    private readonly IEmailService _emailService;

    public OnlineVisitorCreatingSendOTPEventHandler(IOTPService otpService, ISMSService smsService, IEmailService emailService)
    {
        _otpService = otpService;
        _smsService = smsService;
        _emailService = emailService;
    }

    public async Task Handle(VisitorCreatingEvent notification, CancellationToken cancellationToken)
    {
        var visitor = notification.Visitor;

        if (visitor.OTPVerificationRequired is false) return;

        bool isSriLankanNumber = MobileNumberHelper.IsSriLankaNumber(visitor.MobileNumber);

        string sendTo = isSriLankanNumber ? visitor.MobileNumber! : visitor.Email!;

        var response = await _otpService.GenerateOTP(OTPType.NewVisitorOnlineRegistration,
                                                     visitor.NICNumber! ?? visitor.PassportNumber!,
                                                     sendTo);



        if (response.Success is false)
        {
            var error = response.Errors.FirstOrDefault();
            throw new OperationFailedException(error.Key ?? "OTP", error.Value?.FirstOrDefault() ?? "Failed to generate OTP");
        }

        await SendOTPNotification();

        async Task<ResponseResult> SendOTPNotification()
        {
            if (isSriLankanNumber is false)
            {
                string template = await _emailService.GetEmailTemplate("OTPEmailTemplate.html");

                await _emailService.SendEmailByQueue(EmailBuilder.BuildOTP(sendTo,
                                                                           response.Data!.Code,
                template));
                return new();
            }

            else
            {
                await _smsService.SendSMSByQueue(SMSMessage.BuildOTPMessage(sendTo, response.Data!.Code));
                return new();
            }
        }
    }
}
