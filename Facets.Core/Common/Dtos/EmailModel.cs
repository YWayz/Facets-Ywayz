using System.Net.Mail;

namespace Facets.Core.Common.Dtos;

public sealed class EmailModel
{
    public string Body { get; init; } = null!;
    public string To { get; init; } = null!;
    public string Subject { get; init; } = null!;
    public List<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();
}
public class EmailAttachment
{
    public string FileName { get; set; }
    public byte[] Content { get; set; }
    public string ContentType { get; set; }
}
