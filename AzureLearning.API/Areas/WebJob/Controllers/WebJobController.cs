using Asp.Versioning;
using AzureLearning.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;

namespace AzureLearning.API.Areas.WebJob.Controllers
{
    [Route("api/v{version:apiVersion}/webJob")]
    [ApiVersion("1.0")]
    [ApiController]
    public class WebJobController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public WebJobController(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Trigger Azure WebJob Manually
        /// </summary>
        [HttpGet("trigger")]
        public async Task<ApiPostResponse<string>> TriggerWebJob()
        {
            var response = new ApiPostResponse<string>();

            try
            {
                string kuduUser = "$AzureLearningAPI";
                string kuduPass = "hGH56Jdd4TMntSri2hZ9qwZjmuayKcs6tRujg5XwekzgjJkfT4NZJ0RfobRb";
                string triggerUrl = "https://azurelearningapi-b9hma5bdfjeygwen.scm.centralindia-01.azurewebsites.net:443/api/triggeredwebjobs/AzureWebJob/run";

                if (string.IsNullOrWhiteSpace(triggerUrl))
                {
                    response.Success = false;
                    response.Message = "WebJob_TriggerUrl not configured.";
                    return response;
                }

                var client = _httpClientFactory.CreateClient();

                // Create Basic Auth header
                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{kuduUser}:{kuduPass}"));
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", authToken);

                // POST (required to run WebJob)
                var result = await client.PostAsync(triggerUrl, null);

                if (result.IsSuccessStatusCode)
                {
                    response.Success = true;
                    response.Message = "WebJob triggered successfully.";
                    response.Data = "AzureWebJob is now running.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Failed to trigger WebJob.";
                    response.Data = await result.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }
    }
}
