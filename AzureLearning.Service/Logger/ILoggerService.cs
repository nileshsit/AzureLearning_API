using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureLearning.Service.Logger
{
    public interface ILoggerService
    {
        Task<bool> LogResponseAsync(string response, string logFileName);
    }
}
