using PhotoService.Domain.Entities;

namespace PhotoService.Domain.Interfaces
{
    public interface IPhotoLikeRepository
    {
        Task<PhotoLike> AddLikeAsync(PhotoLike photoLike);
        Task<IEnumerable<PhotoLike>> GetLikesByPhotoGuidAsync(Guid photoGuid);
        Task<bool> RemoveLikeAsync(Guid photoGuid, Guid userGuid);
    }
}
