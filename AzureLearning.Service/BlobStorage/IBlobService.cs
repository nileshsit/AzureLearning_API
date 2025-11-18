namespace AzureLearning.Service.BlobStorage
{
    public interface IBlobService
    {
        Task<IEnumerable<string>> ListBlobsAsync();
        Task UploadBlobAsync(string name, Stream data, string contentType);
        Task<Stream> DownloadBlobAsync(string name);
        Task DeleteBlobAsync(string name);
        Task<string> GetBlobSasUrlAsync(string name,int expiryMinutes);
    }
}
