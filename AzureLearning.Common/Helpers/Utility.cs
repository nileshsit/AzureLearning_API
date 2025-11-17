using System.ComponentModel;
using System.Net.Mail;
using System.Reflection;

namespace AzureLearning.Common.Helpers
{
    public static class Utility
    {

        /// <summary>
        /// Check if valid email address or not
        /// </summary>
        /// <param name="emailaddress"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check if valid mobile number or not (As per Norway standards)
        /// </summary>
        /// <param name="mobileNumber">Mobile number</param>
        /// <returns></returns>
        public static bool IsValidMobilNo(string mobileNumber)
        {
            bool isValid = false;

            if (mobileNumber.Length > 0 && mobileNumber.All(char.IsNumber))
            {
                isValid = true;
            }

            return isValid;
        }

        /// <summary>
        /// Generate random password
        /// </summary>
        /// <returns></returns>
        public static string GeneratePassword()
        {
            // Password should be 8 caharacters - first 6 lower case then 2 didgits
            var stringLower = "abcdefghijklmnopqrstuvwxyz";
            var numeric = "0123456789";
            var password = "";
            var character = "";
            var characters = 0;
            var numerics = 0;
            Random random = new Random();
            while (characters < 6) // 6 characters lower case
            {
                var entity1 = random.Next(0, stringLower.Length - 1);
                characters++;
                character += stringLower.ToCharArray()[entity1];
            }

            while (numerics < 2) // 2 digits
            {
                var entity3 = random.Next(0, numeric.Length - 1);
                numerics++;

                character += numeric.ToCharArray()[entity3];
            }
            password = character;

            return password;
        }

        public static DateTime Convert_FromUTC(DateTime utcDateTime, string timeZone)
        {
            try
            {
                TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                DateTime dateTimeAsTimeZone = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, nzTimeZone);
                return dateTimeAsTimeZone;
            }
            catch
            {
                throw;
            }
        }

        public static DateTime ConvertToUTC(DateTime dateTime, string timeZone)
        {
            try
            {
                TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, nzTimeZone);
                return utcDateTime;
            }
            catch
            {
                throw;
            }
        }

        public static int[] ConvertEnumToIntArray(Type enums)
        {
            return Enum.GetValues(enums)
           .Cast<int>()
           .Select(x => x)
           .ToArray();
        }

        public static string ApplicationPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public static DateTime ConvertFromUTC(DateTime dateTime, string desiredOffset)
        {
            try
            {
                IReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                TimeZoneInfo desiredTimeZone = null;
                foreach (TimeZoneInfo timeZone in timeZones)
                {
                    if (timeZone.Id == desiredOffset)
                    {
                        desiredTimeZone = timeZone;
                        break;
                    }
                }
                if (desiredTimeZone != null)
                {
                    TimeZoneInfo gmtTimeZone = TimeZoneInfo.FindSystemTimeZoneById(desiredOffset); // Replace with the desired time zone ID for GMT
                    DateTime gmtDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, gmtTimeZone);
                    return gmtDateTime;
                }
                else
                {
                    throw new Exception("Desired time zone not found.");
                }
            }
            catch
            {
                throw;
            }
        }

        public static string GetEncryptPassword(string password)
        {

            string hashed = EncryptionDecryption.Hash(EncryptionDecryption.GetEncrypt(password));
            string[] segments = hashed.Split(":");
            string EncryptedHash = EncryptionDecryption.GetEncrypt(segments[0]);
            string EncryptedSalt = EncryptionDecryption.GetEncrypt(segments[1]);

            return EncryptedHash +"||"+ EncryptedSalt;
        }
        public static string GetDisplayName(PropertyInfo propertyInfo)
        {
            if (propertyInfo != null)
            {
                DisplayNameAttribute? displayNameAttribute = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
                return displayNameAttribute?.DisplayName ?? propertyInfo.Name;
            }
            return "-";
        }
    }
}