using Asp.Versioning;
using AzureLearning.Common.Helpers;
using AzureLearning.Service.BlobStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureLearning.API.Areas.KeyVault.Controllers
{
    [Route("api/v{version:apiVersion}/key-vault")]
    [ApiVersion("1.0")]
    [ApiController]
    public class KeyVaultController : ControllerBase
    {
        private readonly IBlobService _blobService;

        public KeyVaultController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet("sas-url/{name}/{expiryMinutes}")]
        public async Task<ApiPostResponse<string>> GetBlobSasUrl(string name, int expiryMinutes = 60)
        {
            ApiPostResponse<string> response = new();

            var sasUrl = await _blobService.GetBlobSasUrlAsync(name, expiryMinutes);
            response.Success = true;
            response.Data = sasUrl;
            response.Message = $"SAS URL created (valid for {expiryMinutes} minutes).";


            return response;
        }
    }
}
