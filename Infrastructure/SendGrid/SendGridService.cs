using SendGrid;
using SendGrid.Helpers.Mail;

namespace CatalogoZap.Infrastructure.SendGrid;

public interface ISendGridService
{
	Task<bool> SendEmail(string email, string subject, string plainTextContent, string htmlBody);
}

public class SendGridService : ISendGridService
{
    private readonly IConfiguration _config;
	public SendGridService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> SendEmail(string email, string subject, string plainTextContent, string htmlBody)
    {
        string apiKey = _config["SENDGRID_APIKEY"] ?? throw new Exception("SENDGRID_APIKEY is null");
        var client = new SendGridClient(apiKey);

        var from = new EmailAddress(_config["SENDGRID_EMAIL"], "Catalogozap"); 
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
