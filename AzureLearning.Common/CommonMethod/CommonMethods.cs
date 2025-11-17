using System.Web;
using System.Text.RegularExpressions;
using AzureLearning.Common.Helpers;
using AzureLearning.Model.ReqResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AzureLearning.Common.CommonMethod
{
    public class CommonMethods
    {
        #region GetKeyValues
        /// <summary>
        /// Get key value pair result
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ParamValue GetKeyValues(HttpContext context)
        {
            ParamValue paramValues = new ParamValue();
            var headerValue = string.Empty;
            var queryString = string.Empty;
            var jsonString = string.Empty;
            StringValues outValue = string.Empty;

            // for from header value
            if (context.Request.Headers.TryGetValue(Constants.RequestModel, out outValue))
            {
                headerValue = outValue.FirstOrDefault();
                JObject jsonobj = JsonConvert.DeserializeObject<JObject>(headerValue);
                if (jsonobj != null)
                {
                    Dictionary<string, string> keyValueMap = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, JToken> keyValuePair in jsonobj)
                    {
                        keyValueMap.Add(keyValuePair.Key, keyValuePair.Value.ToString());
                    }
                    List<ReqResponseKeyValue> keyValueMapNew = keyValueMap.ToList().Select(i => new ReqResponseKeyValue
                    {
                        Key = i.Key,
                        Value = i.Value
                    }).ToList();
                    jsonString = JsonConvert.SerializeObject(keyValueMapNew);
                }
            }

            // for from query value
            if (context.Request.QueryString.HasValue)
            {
                var dict = HttpUtility.ParseQueryString(context.Request.QueryString.Value);
                queryString = System.Text.Json.JsonSerializer.Serialize(
                                    dict.AllKeys.ToDictionary(k => k, k => dict[k]));
            }


            paramValues.HeaderValue = jsonString;
            paramValues.QueryStringValue = queryString;
            return paramValues;

        }
        #endregion

        public static string GenerateNewRandom()
        {
            Random generator = new Random();
            string r = generator.Next(0, 1000000).ToString("D6");
            if (r.Distinct().Count() == 1)
            {
                r = GenerateNewRandom();
            }
            return r;
        }

        public static bool IsValidEmail(string email)
        {
            // Regular expression pattern for email validation
            const string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return Regex.IsMatch(email, pattern);
        }
        public static bool IsPasswordStrong(string CreatePassword)
        {
            const string pattern = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$"
;

             return Regex.IsMatch(CreatePassword, pattern);
        }
        public static string GetHost(string password)
        {
            //string email = "chirag.p@shaligraminfotec.com";
            //string password = "admin@123";



            string hashed = EncryptionDecryption.Hash(EncryptionDecryption.GetEncrypt(password));



            string[] segments = hashed.Split(":");



            string EncryptedHash = EncryptionDecryption.GetEncrypt(segments[0]);
            string EncryptedSalt = EncryptionDecryption.GetEncrypt(segments[1]);




            string Hash = EncryptionDecryption.GetDecrypt(EncryptedHash);
            string Salt = EncryptionDecryption.GetDecrypt(EncryptedSalt);



            return  EncryptedHash +" || " + EncryptedSalt;
        }
        public static async Task<string> UploadImage(IFormFile userProfile, string Imagepath)
        {
            Guid guidFile = Guid.NewGuid();
            string FileName;
            string BasePath;
            string path;
            string Photo = string.Empty;
            FileName = guidFile + Path.GetExtension(userProfile.FileName);
            BasePath = Path.Combine(Directory.GetCurrentDirectory(), Imagepath);
            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
            path = Path.Combine(BasePath, FileName);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await userProfile.CopyToAsync(stream);
            }
            Photo = FileName;
            return Photo;
        }
    }
}