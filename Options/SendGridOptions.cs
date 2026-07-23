namespace CatalogoZap.Options.SendGrid;

public sealed class SendGridOptions
{
    public string SendGridApiKey { get; set; } = string.Empty;
    public string SendGridEmail{ get; set; } = string.Empty;
}