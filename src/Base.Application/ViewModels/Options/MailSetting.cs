namespace Base.Application.ViewModels.Options;

public class MailSetting
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? MailServerAddress { get; set; }
    public int MailServerPort { get; set; }
}