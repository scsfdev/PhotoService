using Microsoft.EntityFrameworkCore;
using PhotoService.Domain.Entities;
using PhotoService.Infrastructure.Data;
using PhotoService.Domain.Interfaces;

namespace PhotoService.Infrastructure.Repositories
{
    public class PhotoRepository(PhotoDbContext db) : IPhotoRepository
    {
        public async Task<IEnumerable<Photo>> GetAllPhotosAsync() => await db.Photos.Include(p=>p.PhotoLikes).Include(p=>p.PhotoCategories).ToListAsync();

        public async Task<Photo?> GetPhotoByGuidAsync(Guid guid)
        {
            return await db.Photos.Include(p => p.PhotoLikes).Include(p => p.PhotoCategories).FirstOrDefaultAsync(p => p.PhotoGuid == guid);
        }


        public async Task<Photo> AddPhotoAsync(Photo photo)
        {
            await db.Photos.AddAsync(photo);
            await db.SaveChangesAsync();
            return photo;
        }

        public async Task<bool> DeletePhotoAsync(Photo photo)
        {
            db.Photos.Remove(photo);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePhotoAsync(Photo photo)
        {
            var existing = await db.Photos.FirstOrDefaultAsync(p => p.PhotoGuid == photo.PhotoGuid);
            if(existing == null)
            {
                return false;
            }

            await db.SaveChangesAsync();
            return true;
        }

        public async Task IncrementLikesAsync(Guid photoGuid)
        {
            var photo = await db.Photos.FirstOrDefaultAsync(p => p.PhotoGuid == photoGuid);
            if (photo != null)
            {
                photo.LikesCount++;
                await db.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<List<Photo>> GetPhotosPaginatedAsync(int pageNumber, int pageSize)
        {
            return await db.Photos
                .Include(p => p.PhotoLikes)
                .Include(p => p.PhotoCategories)
                .OrderByDescending(p => p.UploadedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalPhotoCountAsync()
        {
            return await db.Photos.CountAsync();
        }
    }
}
