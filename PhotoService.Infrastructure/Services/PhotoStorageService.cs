using PhotoService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoService.Infrastructure.Services
{
    public class PhotoStorageService(IStorageService storageService) : IPhotoStorageService
    {
        public string GetPhotoSignedUrl(string fileName, int validMins = 60)
        {
            return storageService.GetSignedUrl(fileName, validMins);
        }

        public List<string> GetSignedUrls(IEnumerable<string> fileNames, int validMins = 60)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            return await storageService.UploadFileAsync(fileStream, fileName, contentType);
        }
    }
}
