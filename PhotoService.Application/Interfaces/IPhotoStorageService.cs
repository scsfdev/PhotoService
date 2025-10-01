
namespace PhotoService.Application.Interfaces
{
    public interface IPhotoStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
        string GetPhotoSignedUrl(string fileName, int validMins = 60);
        List<string> GetSignedUrls(IEnumerable<string> fileNames, int validMins = 60);
    }
}
