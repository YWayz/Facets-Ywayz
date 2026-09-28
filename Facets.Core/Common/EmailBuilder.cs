using Facets.Core.Common.Dtos;
using System.Data.SqlTypes;
using System.Net.Mail;
using System.Text;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Common;

public sealed class EmailBuilder
{
    // Visitor-typed values go into HTML templates; encode them so a name cannot inject markup or scripts.
    private static string H(string? value) => System.Net.WebUtility.HtmlEncode(value ?? string.Empty);

    public static EmailModel BuildOTP(string toEmailAddress, string code, string template)
    {
        StringBuilder emailBody = new(template);

        emailBody.Replace("#otp_code#", H(code));

        EmailModel email = new()
        {
            To = toEmailAddress,
            Subject = "Facets OTP",
            Body = emailBody.ToString(),
        };

        return email;
    }

    public static EmailModel BuildEventRegistrationCompletedMessage(string toEmailAddress,
                                                                    string eventName,
                                                                    string visitorFirstName,
                                                                    string visitorLastName,
                                                                    string visitorReference,
                                                                    VisitorIdentityType visitorIdentityType,
                                                                    string identificationNumber,
                                                                    string commaDelimeteredDates,
                                                                    string template, List<byte[]> svgBytesList)
    {
        StringBuilder emailBody = new(template);

        emailBody
        .Replace("#event_name#", H(eventName))
        .Replace("#visitor_reference#", H(visitorReference))
        .Replace("#visitor_first_name#", H(visitorFirstName))
        .Replace("#visitor_last_name#", H(visitorLastName))
        .Replace("#visitor_identity_type#", visitorIdentityType.ToString())
        .Replace("#identification_number#", H(identificationNumber))
        .Replace("#comma_delimetered_dates#", H(commaDelimeteredDates));

        EmailModel email = new()
        {
            To = toEmailAddress,
            Subject = "Facets Sri Lanka - Event Registration Details",
            Body = emailBody.ToString(),
        };
        // Add the SVG attachment
        if (svgBytesList != null && svgBytesList.Count > 0)
        {
            for (int i = 0; i < svgBytesList.Count; i++)
            {
                // Generate a unique filename for each attachment
                string fileName = $"event-qr-{i + 1}.svg";
                email.Attachments.Add(new EmailAttachment
                {
                    FileName = fileName,
                    Content = svgBytesList[i],
                    ContentType = "image/svg+xml"
                });
            }
        }

        return email;
    }

    public static EmailModel BuildCancellationEmailMessage(string toEmailAddress,
                                                                   string eventName,
                                                                   string visitorFirstName,
                                                                   string visitorLastName,
                                                                   string visitorReference,
                                                                   VisitorIdentityType visitorIdentityType,
                                                                   string identificationNumber,
                                                                   string commaDelimeteredDates,
                                                                   string template)
    {
        StringBuilder emailBody = new(template);

        emailBody
        .Replace("#event_name#", H(eventName))
        .Replace("#visitor_reference#", H(visitorReference))
        .Replace("#visitor_first_name#", H(visitorFirstName))
        .Replace("#visitor_last_name#", H(visitorLastName))
        .Replace("#visitor_identity_type#", visitorIdentityType.ToString())
        .Replace("#identification_number#", H(identificationNumber))
        .Replace("#comma_delimetered_dates#", H(commaDelimeteredDates));

        EmailModel email = new()
        {
            To = toEmailAddress,
            Subject = "Facets Sri Lanka - Event Registration Details",
            Body = emailBody.ToString(),
        };
        return email;
    }
}
