using System.Net.Mail;
using System.Net;
using PartsControlSystem.Services;

public class MailService : IMailService
{
    private readonly IConfiguration _config;

    public MailService(IConfiguration config)
    {
        _config = config;
    }

    // Original method
    public async Task SendEmailAsync(string toEmail, string subject, string body)
        => await SendEmailAsync(toEmail, subject, body, null);

    // Original method with attachments
    public async Task SendEmailAsync(string toEmail, string subject, string body, List<(byte[] fileBytes, string fileName)> attachments)
    {
        await SendEmailAsync(new List<string> { toEmail }, subject, body, attachments);
    }

    // New method: multiple recipients
    public async Task SendEmailAsync(IEnumerable<string> toEmails, string subject, string body, List<(byte[] fileBytes, string fileName)> attachments)
    {
        var smtpSettings = _config.GetSection("EmailSettings");
        var host = smtpSettings["Host"];
        var port = int.Parse(smtpSettings["Port"]);
        var enableSsl = bool.Parse(smtpSettings["EnableSSL"]);
        var username = smtpSettings["Username"];
        var password = smtpSettings["Password"];

        System.Net.ServicePointManager.Expect100Continue = false;

        using var smtpClient = new SmtpClient(host)
        {
            Port = port,
            EnableSsl = enableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Timeout = 60000
        };

        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            smtpClient.Credentials = new NetworkCredential(username, password);
        }

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpSettings["SenderEmail"], smtpSettings["SenderName"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        // Add multiple recipients
        foreach (var email in toEmails)
        {
            mailMessage.To.Add(email);
        }

        // Attachments
        if (attachments != null)
        {
            foreach (var file in attachments)
            {
                var stream = new MemoryStream(file.fileBytes);
                stream.Position = 0;
                mailMessage.Attachments.Add(new Attachment(stream, file.fileName));
            }
        }

        await smtpClient.SendMailAsync(mailMessage);
    }
}
