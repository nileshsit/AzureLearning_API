using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureLearning.Model.Settings
{
    public class AppSettings
    {
        public string? JWT_Secret { get; set; }
        public int JWT_Validity_Mins { get; set; }
        public int PasswordLinkValidityMins { get; set; }
        public string? MailChimpApiKey { get; set; }
        public string? MailChimpbaseURL { get; set; }
        public string ErrorEmail { get; set; }
        public string ErrorSendToEmail { get; set; }
        public int ForgotPasswordAttemptValidityHours { get; set; }
    }
}
