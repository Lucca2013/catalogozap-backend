using SendGrid;
using SendGrid.Helpers.Mail;
using CatalogoZap.Options.SendGrid;
using Microsoft.Extensions.Options;

namespace CatalogoZap.Infrastructure.SendGrid;

public sealed class SendGridService(IOptions<SendGridOptions> options)
{
    public async Task<bool> SendEmail(string email, string subject, string plainTextContent, string htmlBody)
    {
        string apiKey = options.Value.SendGridApiKey;
        var client = new SendGridClient(apiKey);

        var from = new EmailAddress(options.Value.SendGridEmail, "Catalogozap"); 
        var to = new EmailAddress(email, email);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlBody);

        var response = await client.SendEmailAsync(msg);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            Console.WriteLine($"Error at sending email: {response.StatusCode}");
            var responseBody = await response.Body.ReadAsStringAsync();
            Console.WriteLine($"Error details: {responseBody}");

            return false;
        }
    }
}
