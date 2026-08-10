namespace Facets.FunctionApp.CP.NotificationFunctions.Models;

public sealed class EmailMessage
{
    public string Body { get; set; } = null!;
    public string To { get; set; } = null!;
    public string Subject { get; set; } = null!; 
    public List<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();
}
public class EmailAttachment
{
    public string FileName { get; set; }
    public byte[] Content { get; set; }
    public string ContentType { get; set; }
}
