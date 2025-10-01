using PhotoService.Application.DTOs;
using PhotoService.Domain.Entities;

namespace PhotoService.Application.Interfaces
{
    public interface IPhotoService
    {
        Task<IEnumerable<PhotoDto>> GetAllPhotosAsync();
        Task<PhotoDto?> GetPhotoByGuidAsync(Guid guid);

        Task<PhotoDto> AddPhotoAsync(PhotoCreateDto photo);
        Task<bool> UpdatePhotoAsync(PhotoWriteFormDto photo);
        Task<bool> DeletePhotoAsync(Guid photoGuid);

        Task<PhotoLikeDto> LikePhotoAsync(Guid photoGuid, Guid userGuid);
        Task<bool> UnlikePhotoAsync(Guid photoGuid, Guid userGuid);


        Task<IEnumerable<PhotoCategoryDto>> GetPhotoCategoryByPhotoGuidAsync(Guid photoGuid);

        Task<bool> CreatePhotoCategoryAsync(PhotoCategoryDto photoCategory);
        Task<bool> DeletePhotoCategoryAsync(Guid photoGuid, Guid categoryGuid);

        Task<PagedResult<PhotoDto>> GetPhotosPaginatedAsync(int pageNumber, int pageSize);
    }


}
