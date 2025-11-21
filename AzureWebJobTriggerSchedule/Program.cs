using AzureWebJobTriggerSchedule.EmailNotification;

Console.WriteLine("WebJob started...");

// Load email settings from Azure App Settings
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

// Multiple recipients
string recipients = Environment.GetEnvironmentVariable("ToEmails");

// --------------------------------------------------
// Load HTML Template from WebJob local folder
// --------------------------------------------------
string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "NotificationTemplate.html");

string emailBody = File.Exists(templatePath)
                    ? await File.ReadAllTextAsync(templatePath)
                    : "<h2>WebJob Schedule</h2><p>No template found.</p>";

// Replace tokens in HTML (if any)
emailBody = emailBody.Replace("##DateTime##", DateTime.Now.ToString());
emailBody = emailBody.Replace("##Message##", "Schedule event successfully processed.");
emailBody = emailBody.Replace("##LogoURL##", "https://brandlogos.net/wp-content/uploads/2015/08/microsoft_azure_2012-2014-logo_brandlogos.net_qfajw.png");

// --------------------------------------------------
// Send Email
// --------------------------------------------------

bool success = await EmailNotification.SendAsyncEmail(
    recipient: recipients,
    bcc: null,
    cc: null,
    subject: "Schedule Notification",
    body: emailBody,
    emailSetting: emailSetting,
    attachment: null
);

if (success)
{
    Console.WriteLine("Email sent successfully!");
}
else
{
    Console.WriteLine("Failed to send email.");
}
