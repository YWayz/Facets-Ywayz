namespace Facets.FunctionApp.CP.NotificationFunctions.Models;

public sealed record SMSMessage(string MobileNumber, string Message, string Subject);
