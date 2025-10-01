using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System.ComponentModel.DataAnnotations;


namespace PhotoService.Infrastructure.Services
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
        string GetSignedUrl(string fileName, int validMinutes);
        List<string> GetSignedUrls(IEnumerable<string> fileNames, int validMinutes);
    }

    public class GCSService : IStorageService
    {
        private readonly string bucketName = "my-photo-album";
        private readonly StorageClient storageClient = null!;
        private readonly UrlSigner urlSigner = null!;

       
        public GCSService()
        {
            storageClient = StorageClient.Create();

            var jsonPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

            // Load service account credential
            var credential = GoogleCredential.FromFile(jsonPath).UnderlyingCredential as ServiceAccountCredential;
            urlSigner = UrlSigner.FromCredential(credential);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var obj = await storageClient.UploadObjectAsync(bucket: bucketName, objectName: fileName, contentType: contentType, source: fileStream);

            // Return image URL.
            return $"https://storage.googleapis.com/{bucketName}/{fileName}";
        }

        public string GetSignedUrl(string fileName, int validMinutes = 60)
        {
            return urlSigner.Sign(
                bucket: bucketName,
                objectName: fileName,
                duration: TimeSpan.FromMinutes(validMinutes),
                HttpMethod.Get
            );
        }

        public List<string> GetSignedUrls(IEnumerable<string> fileNames, int validMinutes = 60)
        {
            var urls = new List<string>();
            foreach (var file in fileNames)
            {
                urls.Add(GetSignedUrl(file, validMinutes));
            }
            return urls;
        }
    }
}
