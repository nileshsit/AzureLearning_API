using AzureWebJobContinuous.EmailNotification;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Continuous WebJob started...");

        // Cancellation token to handle graceful shutdown
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        // Handle App Service shutdown
        AppDomain.CurrentDomain.ProcessExit += async (s, e) =>
        {
            Console.WriteLine("App Service stopping...");
            await SendEmailAsync("WebJob Stopped", "The WebJob has stopped as the App Service is shutting down.");
        };

        // Send startup email
        await SendEmailAsync("WebJob Started", "The WebJob has started as the App Service has started.");

        // Continuous loop: heartbeat email every minute
        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                await SendEmailAsync("WebJob Running", "This is a heartbeat notification. The WebJob is running normally.");
                await Task.Delay(TimeSpan.FromMinutes(1), cts.Token); // wait 1 minute
            }
            catch (TaskCanceledException)
            {
                // Graceful exit
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in loop: {ex.Message}");
            }
        }

        Console.WriteLine("Continuous WebJob exiting...");
    }

    private static async Task SendEmailAsync(string subject, string message)
    {
        try
        {
            // Load email settings from environment variables
            var emailSetting = new EmailNotification.EmailSetting
            {
                EmailHostName = Environment.GetEnvironmentVariable("EmailHostName"),
                EmailPort = int.Parse(Environment.GetEnvironmentVariable("EmailPort") ?? "587"),
                EmailEnableSsl = bool.Parse(Environment.GetEnvironmentVariable("EmailEnableSsl") ?? "true"),
                EmailUsername = Environment.GetEnvironmentVariable("EmailUsername"),
                EmailAppPassword = Environment.GetEnvironmentVariable("EmailAppPassword"),
                FromEmail = Environment.GetEnvironmentVariable("FromEmail"),
                FromName = Environment.GetEnvironmentVariable("FromName")
            };

            string recipients = Environment.GetEnvironmentVariable("ToEmails");

            // Load HTML template
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "NotificationTemplate.html");
            string emailBody = File.Exists(templatePath)
                ? await File.ReadAllTextAsync(templatePath)
                : $"<h2>{subject}</h2><p>{message}</p>";

            // Replace placeholders
            emailBody = emailBody.Replace("##DateTime##", DateTime.Now.ToString());
            emailBody = emailBody.Replace("##Message##", message);
            emailBody = emailBody.Replace("##LogoURL##", "https://brandlogos.net/wp-content/uploads/2015/08/microsoft_azure_2012-2014-logo_brandlogos.net_qfajw.png");

            // Send email
            bool success = await EmailNotification.SendAsyncEmail(
                recipient: recipients,
                bcc: null,
                cc: null,
                subject: subject,
                body: emailBody,
                emailSetting: emailSetting,
                attachment: null
            );

            Console.WriteLine(success ? $"Email sent: {subject}" : $"Failed to send email: {subject}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}
