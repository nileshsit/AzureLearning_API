using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Web;

namespace DemoAzureFunction
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public HttpResponseData run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }

        [Function("Function2")]
        public async Task<HttpResponseData> Function2Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("Function2 executed.");

            // 1. Read query parameter
            var query = HttpUtility.ParseQueryString(req.Url.Query);
            string name = query["name"];

            // 2. Read body
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            if (!string.IsNullOrEmpty(body))
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
                if (data != null && data.ContainsKey("name"))
                    name = data["name"];
            }

            // 3. Prepare response
            string responseMessage = string.IsNullOrEmpty(name) ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response." : $"Hello, {name}. This HTTP triggered function executed successfully.";

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(responseMessage);
            return response;
        }
    }
}
