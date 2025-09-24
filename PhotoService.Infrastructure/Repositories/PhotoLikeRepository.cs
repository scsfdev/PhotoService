using Microsoft.EntityFrameworkCore;
using PhotoService.Domain.Entities;
using PhotoService.Infrastructure.Data;
using PhotoService.Domain.Interfaces;

namespace PhotoService.Infrastructure.Repositories
{
    internal class PhotoLikeRepository(PhotoDbContext db) : IPhotoLikeRepository
    {
        public async Task<PhotoLike> AddLikeAsync(PhotoLike photoLike)
        {
            db.PhotoLikes.Add(photoLike);
            await db.SaveChangesAsync();
            return photoLike;
        }

        public async Task<IEnumerable<PhotoLike>> GetLikesByPhotoGuidAsync(Guid photoGuid) =>
            await db.PhotoLikes.Where(pl => pl.PhotoGuid == photoGuid).ToListAsync();

        public async Task<bool> RemoveLikeAsync(Guid photoGuid, Guid userGuid)
        {
            var like = await db.PhotoLikes.FirstOrDefaultAsync(pl => pl.PhotoGuid == photoGuid && pl.UserGuid == userGuid);

            if (like == null)
                return false;

            db.PhotoLikes.Remove(like);
            await db.SaveChangesAsync();
            return true;
        }
    }
}
