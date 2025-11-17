using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureLearning.Common.Helpers
{
    public class Constants
    {
        public const string encryptionAlgorythm = "MD5";
        public const string ContentTypeJson = "application/json";
        public const string RequestModel = "requestModel";
        public const string AccessControlAllowOrigin = "*";
        public const string LogsFilePathException = "Logs/ExceptionLogs";
        public const string LogsFilePathRequest = "Logs/RequestLogs";
        public const string ExceptionReportPath = "wwwroot\\EmailTemplates";
        public const string ExceptionReport = "ExceptionReport.html";
    }
}
