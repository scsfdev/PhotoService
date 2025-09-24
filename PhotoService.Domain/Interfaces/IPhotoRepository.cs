using PhotoService.Domain.Entities;

namespace PhotoService.Domain.Interfaces
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<Photo>> GetAllPhotosAsync();
        Task<Photo?> GetPhotoByGuidAsync(Guid guid);

        Task<Photo> AddPhotoAsync(Photo photo);
        Task<bool> UpdatePhotoAsync(Photo photo);
        Task<bool> DeletePhotoAsync(Photo photo);
        Task<bool> SaveChangesAsync();
        Task IncrementLikesAsync(Guid photoGuid);
    }
}
