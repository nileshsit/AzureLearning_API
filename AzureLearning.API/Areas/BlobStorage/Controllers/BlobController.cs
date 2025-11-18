using Asp.Versioning;
using AzureLearning.Common.Helpers;
using AzureLearning.Model.BlobStorage;
using AzureLearning.Service.BlobStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureLearning.API.Areas.BlobStorage.Controllers
{
    [Route("api/v{version:apiVersion}/blob")]
    [ApiVersion("1.0")]
    [ApiController]
    public class BlobController : ControllerBase
    {
        private readonly IBlobService _blobService;


        public BlobController(IBlobService blobService)
        {
            _blobService = blobService;
        }


        [HttpGet("list")]
        public async Task<ApiResponse<string>> List()
        {
            ApiResponse<string> response = new() { Data = new List<string>() };

            var blobs = await _blobService.ListBlobsAsync();
            response.Data = blobs?.ToList() ?? new System.Collections.Generic.List<string>();
            response.Success = true;
            response.Message = "Blobs retrieved successfully.";

            return response;
        }


        [HttpPost("upload")]
        public async Task<ApiPostResponse<string>> Upload([FromForm] BlobUploadRequestModel request)
        {
            ApiPostResponse<string> response = new();

            if (request.File == null || request.File.Length == 0)
            {
                response.Success = false;
                response.Message = "No file uploaded.";
                return response;
            }

            using var stream = request.File.OpenReadStream();
            await _blobService.UploadBlobAsync(request.File.FileName, stream, request.File.ContentType);

            response.Success = true;
            response.Data = request.File.FileName;
            response.Message = "File uploaded successfully.";

            return response;
        }


        [HttpGet("download/{name}")]
        public async Task<IActionResult> Download(string name)
        {
            var stream = await _blobService.DownloadBlobAsync(name);
            return File(stream, "application/octet-stream", name);
        }


        [HttpDelete("{name}")]
        public async Task<BaseApiResponse> Delete(string name)
        {
            BaseApiResponse response = new();

            await _blobService.DeleteBlobAsync(name);
            response.Success = true;
            response.Message = "Blob deleted successfully.";

            return response;
        }

        [HttpGet("sas-url/{name}/{expiryMinutes}")]
        public async Task<ApiPostResponse<string>> GetBlobSasUrl(string name,int expiryMinutes = 60)
        {
            ApiPostResponse<string> response = new();

                var sasUrl = await _blobService.GetBlobSasUrlAsync(name, expiryMinutes);
                response.Success = true;
                response.Data =   sasUrl;
                response.Message = $"SAS URL created (valid for {expiryMinutes} minutes).";
            

            return response;
        }
    }
}
