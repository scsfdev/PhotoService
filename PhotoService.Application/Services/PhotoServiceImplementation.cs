using AutoMapper;
using PhotoService.Application.DTOs;
using PhotoService.Application.Interfaces;
using PhotoService.Domain.Entities;
using PhotoService.Domain.Interfaces;

namespace PhotoService.Application.Services
{
    public class PhotoServiceImplementation(IPhotoRepository photoRepository, IPhotoLikeRepository photoLikeRepository, 
        IPhotoCategoryRepository photoCategoryRepository, IMapper mapper) : IPhotoService
    {
        // Photo operations ----------------------------------------------------------------------------
        public async Task<IEnumerable<PhotoDto>> GetAllPhotosAsync()
        {
            var photos = await photoRepository.GetAllPhotosAsync();
            return mapper.Map<IEnumerable<PhotoDto>>(photos);
        }

        public async Task<PhotoDto?> GetPhotoByGuidAsync(Guid guid)
        {
            var photo = await photoRepository.GetPhotoByGuidAsync(guid);
            if(photo == null)
                return null;
            return mapper.Map<PhotoDto>(photo);
        }

        public async Task<PhotoDto> AddPhotoAsync(PhotoCreateDto photoDto)
        {
            var photo = mapper.Map<Photo>(photoDto);
            await photoRepository.AddPhotoAsync(photo);
            return mapper.Map<PhotoDto>(photo);
        }

        public async Task<bool> UpdatePhotoAsync(PhotoUpdateDto photoDto)
        {
            var existingPhoto = await photoRepository.GetPhotoByGuidAsync(photoDto.PhotoGuid);
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

        public async Task<bool> DeletePhotoCategoryAsync(Guid photoGuid, Guid categoryGuid)
        {
            return await photoCategoryRepository.DeleteAsync(photoGuid, categoryGuid);
        }

       

        // Photo Category operations ----------------------------------------------------------------------------
    }
}
