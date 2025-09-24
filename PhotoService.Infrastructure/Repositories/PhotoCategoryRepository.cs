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

        public async Task<bool> DeleteAsync(Guid photoGuid, Guid categoryGuid)
        {
            var photoCategory = db.PhotoCategories.FirstOrDefault(pc => pc.PhotoGuid == photoGuid && pc.CategoryGuid == categoryGuid);
            if(photoCategory != null)
            {
                db.PhotoCategories.Remove(photoCategory);
            }

            return await db.SaveChangesAsync() > 0;
        }
    }
}
