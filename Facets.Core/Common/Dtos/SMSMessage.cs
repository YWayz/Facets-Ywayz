using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Common.Dtos;

public sealed class SMSMessage
{
    public string MobileNumber { get; init; }
    public string Message { get; init; }
    public string Subject { get; }

    private SMSMessage(string mobileNumber, string message, string subject)
    {
        MobileNumber = mobileNumber;
        Message = message;
        Subject = subject;
    }

    public static SMSMessage BuildOTPMessage(string mobileNumber, string code)
    {
        string message = $"Facets verification code is {code}";
        return new(mobileNumber, message, subject: "OTP");
    }

    public static SMSMessage BuildEventRegistrationCompletedMessage(string mobileNumber,
                                                                    string eventName,
                                                                    string visitorFirstName,
                                                                    string visitorLastName,
                                                                    string visitorReference,
                                                                    VisitorIdentityType visitorIdentityType,
                                                                    string identificationNumber,
                                                                    string commaDelimeteredDates)
    {
        string message = $"""
                   Congratulations! You have successfully registered for the {eventName}.
                   
                   Your registration details:
                   
                   Name: {visitorFirstName} {visitorLastName}

                   ID Number: {visitorIdentityType} - {identificationNumber}

                   Event dates: {commaDelimeteredDates}

                   Visitor Reference No: {visitorReference}

                   Thank you for joining us!
                   """;

        return new(mobileNumber, message, subject: "Event Registration Completed");
    }


    public static SMSMessage BuildEventRegistrationCancelCompletedMessage(string mobileNumber,
                                                                    string eventName,
                                                                    string visitorFirstName,
                                                                    string visitorLastName,
                                                                    string visitorReference,
                                                                    VisitorIdentityType visitorIdentityType,
                                                                    string identificationNumber,
                                                                    string commaDelimeteredDates)
    {
        string message = $"""
                   Your registration for {eventName} has been cancelled.
                   
                   Your registration details:
                   
                   Name: {visitorFirstName} {visitorLastName}

                   ID Number: {visitorIdentityType} - {identificationNumber}

                   Event dates: {commaDelimeteredDates} 

                   We're sorry to see you go!
                   """;

        return new(mobileNumber, message, subject: "Event Registration Cancel Completed");
    }
}
