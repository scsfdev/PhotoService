using AutoMapper;
using PhotoService.Application.DTOs;
using PhotoService.Application.Interfaces;
using PhotoService.Domain.Entities;
using PhotoService.Domain.Interfaces;

namespace PhotoService.Application.Services
{
    public class PhotoServiceImplementation(
        IPhotoRepository photoRepository, 
        IPhotoLikeRepository photoLikeRepository, 
        IPhotoCategoryRepository photoCategoryRepository, 
        IPhotoStorageService photoStorageService,
        IMapper mapper) : IPhotoService
    {
        // Photo operations ----------------------------------------------------------------------------
        public async Task<IEnumerable<PhotoDto>> GetAllPhotosAsync()
        {
            var photos = await photoRepository.GetAllPhotosAsync();
            var photoDtos = mapper.Map<IEnumerable<PhotoDto>>(photos);

            foreach (var dto in photoDtos)
            {
                dto.Url = photoStorageService.GetPhotoSignedUrl(dto.FileName, 60);
            }

            return photoDtos;
        }

        public async Task<PhotoDto?> GetPhotoByGuidAsync(Guid guid)
        {
            var photo = await photoRepository.GetPhotoByGuidAsync(guid);
            if(photo == null)
                return null;

            var photoDto = mapper.Map<PhotoDto>(photo);
            photoDto.Url = photoStorageService.GetPhotoSignedUrl(photoDto.FileName, 60);
            return photoDto;
        }

        public async Task<PhotoDto> AddPhotoAsync(PhotoCreateDto photoDto)
        {
            var photo = mapper.Map<Photo>(photoDto);
            await photoRepository.AddPhotoAsync(photo);
            return mapper.Map<PhotoDto>(photo);
        }

        public async Task<bool> UpdatePhotoAsync(PhotoWriteFormDto photoDto)
        {
            if (!photoDto.PhotoGuid.HasValue)
                throw new ArgumentException("PhotoGuid is required for update!");

            var existingPhoto = await photoRepository.GetPhotoByGuidAsync(photoDto.PhotoGuid.Value);
            if (existingPhoto == null)
                return false;

            mapper.Map(photoDto, existingPhoto);

            return await photoRepository.SaveChangesAsync();
        }

        public async Task<bool> DeletePhotoAsync(Guid photoGuid)
        {
            var existing = await photoRepository.GetPhotoByGuidAsync(photoGuid);
            if (existing == null)
                return false;

            return await photoRepository.DeletePhotoAsync(existing);
        }

        // Photo operations ----------------------------------------------------------------------------


        // Photo Like operations ----------------------------------------------------------------------------
        public async Task<PhotoLikeDto> LikePhotoAsync(Guid photoGuid, Guid userGuid)
        {
           var photo = await photoRepository.GetPhotoByGuidAsync(photoGuid) ?? throw new KeyNotFoundException("Photo not found");

            var like = new PhotoLike
            {
                PhotoGuid = photoGuid,
                UserGuid = userGuid,
                LikedAt = DateTime.UtcNow
            };

            await photoLikeRepository.AddLikeAsync(like);

            photo.LikesCount++;
            await photoRepository.UpdatePhotoAsync(photo);

            return mapper.Map<PhotoLikeDto>(like);
        }

        public async Task<bool> UnlikePhotoAsync(Guid photoGuid, Guid userGuid)
        {
            var deleted = await photoLikeRepository.RemoveLikeAsync(photoGuid, userGuid);
            if(!deleted)
                return false;

            var photo = await photoRepository.GetPhotoByGuidAsync(photoGuid);
            if(photo != null && photo.LikesCount > 0)
            {
                photo.LikesCount--;
                await photoRepository.UpdatePhotoAsync(photo);
            }

            return true;
        }

        // Photo Like operations ----------------------------------------------------------------------------



        // Photo Category operations ----------------------------------------------------------------------------
        public async Task<IEnumerable<PhotoCategoryDto>> GetPhotoCategoryByPhotoGuidAsync(Guid photoGuid)
        {
            var photoCategories = await photoCategoryRepository.GetByPhotoGuidAsync(photoGuid);
            return mapper.Map<IEnumerable<PhotoCategoryDto>>(photoCategories);
        }

        public async Task<bool> CreatePhotoCategoryAsync(PhotoCategoryDto photoCategoryDto)
        {
            var photoCategory = mapper.Map<PhotoCategory>(photoCategoryDto);
            return await photoCategoryRepository.CreateAsync(photoCategory);
        }

        public async Task<bool> DeletePhotoCategoryAsync(Guid photoGuid)
        {
            return await photoCategoryRepository.DeleteAsync(photoGuid);
        }

        public async Task<PagedResult<PhotoDto>> GetPhotosPaginatedAsync(int pageNumber, int pageSize)
        {
            var photos = await photoRepository.GetPhotosPaginatedAsync(pageNumber, pageSize);
            var totalCount = await photoRepository.GetTotalPhotoCountAsync();

            return new PagedResult<PhotoDto>
            {
                Items = mapper.Map<List<PhotoDto>>(photos),
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }



        // Photo Category operations ----------------------------------------------------------------------------
    }
}
