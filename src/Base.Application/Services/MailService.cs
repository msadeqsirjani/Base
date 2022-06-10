namespace Base.Application.Services;

public interface IMailService
{
    Task SendAsync(string email, string name, string filename, CancellationToken cancellationToken = new());
}

public class MailService : IMailService
{
    private readonly MailSetting _mailSetting;
    private readonly IWebHostEnvironment _environment;
    private readonly OtpSetting _otpSetting;

    public MailService(IOptions<MailSetting> mailSetting, IWebHostEnvironment environment, IOptions<OtpSetting> otpSetting)
    {
        _mailSetting = mailSetting.Value;
        _environment = environment;
        _otpSetting = otpSetting.Value;
    }

    public async Task SendAsync(string email, string name, string filename, CancellationToken cancellationToken = new())
    {
        if (!email.IsValidEmail())
            throw new MessageException("پست الکترونیکی معتبر نمی باشد");

        MimeMessage message = new();

        MailboxAddress from = new(name, _mailSetting.Email);
        message.From.Add(from);

        MailboxAddress to = new(email, email);
        message.To.Add(to);

        message.Subject = "Your single-use code";

        var singleUseCode = RandomCodeGenerator.GenerateRandomCode(_otpSetting.Length, _otpSetting.PermittedLetters);

        BodyBuilder builder = new()
        {
            HtmlBody = string.Format(
                await File.ReadAllTextAsync(Path.Combine(_environment.WebRootPath, filename), cancellationToken), email, singleUseCode, name)
        };

        message.Body = builder.ToMessageBody();

        using SmtpClient client = new();
        await client.ConnectAsync(_mailSetting.MailServerAddress, _mailSetting.MailServerPort, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(_mailSetting.Email, _mailSetting.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}