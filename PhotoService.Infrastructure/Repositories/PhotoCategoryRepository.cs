using Microsoft.EntityFrameworkCore;
using PhotoService.Domain.Entities;
using PhotoService.Domain.Interfaces;
using PhotoService.Infrastructure.Data;

namespace PhotoService.Infrastructure.Repositories
{
    public class PhotoCategoryRepository(PhotoDbContext db) : IPhotoCategoryRepository
    {
        public async Task<IEnumerable<PhotoCategory>> GetByPhotoGuidAsync(Guid photoGuid)
        {
            return await db.PhotoCategories.Where(pc => pc.PhotoGuid == photoGuid).ToListAsync();
        }

        public async Task<bool> CreateAsync(PhotoCategory photoCategory)
        {
            await db.PhotoCategories.AddAsync(photoCategory);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid photoGuid)
        {
            var existingPhotoCategories = db.PhotoCategories.Where(pc => pc.PhotoGuid == photoGuid);

            if (existingPhotoCategories.Any())
            {
                // If got data, clear it.
                db.PhotoCategories.RemoveRange(existingPhotoCategories);

                return await db.SaveChangesAsync() > 0;
            }

            // Regardless of empty or not empty, return true.
            return true;
        }
    }
}
