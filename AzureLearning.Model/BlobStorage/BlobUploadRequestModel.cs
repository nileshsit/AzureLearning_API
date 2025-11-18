using Microsoft.AspNetCore.Http;

namespace AzureLearning.Model.BlobStorage
{
    public class BlobUploadRequestModel
    {
        public IFormFile File { get; set; }
    }
}
