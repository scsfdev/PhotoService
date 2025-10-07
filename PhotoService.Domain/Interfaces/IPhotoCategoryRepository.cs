using PhotoService.Domain.Entities;

namespace PhotoService.Domain.Interfaces
{
    public interface IPhotoCategoryRepository
    {
        Task<IEnumerable<PhotoCategory>> GetByPhotoGuidAsync(Guid photoGuid);

        Task<bool> CreateAsync(PhotoCategory photoCategory);
        Task<bool> DeleteAsync(Guid photoGuid);
    }
}
