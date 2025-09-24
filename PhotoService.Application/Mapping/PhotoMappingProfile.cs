using AutoMapper;
using PhotoService.Application.DTOs;
using PhotoService.Domain.Entities;

namespace PhotoService.Application.Mapping
{
    public class PhotoMappingProfile: Profile
    {
        public PhotoMappingProfile()
        {
            // Input > Create: Dto to Entity.
            CreateMap<PhotoCreateDto, Photo>()
                .ForAllMembers(opts => opts.Condition((src, dest,srcMember) => srcMember != null));

            // Output > Read: Entity to Dto.
            CreateMap<Photo, PhotoDto>();

            // Input > Update: Dto to Entity.
            CreateMap<PhotoUpdateDto, Photo>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Output > Read: Entity to Dto.
            CreateMap<PhotoLike, PhotoLikeDto>();

            // Output > Read: Entity to Dto.
            CreateMap<PhotoCategory, PhotoCategoryDto>().ReverseMap();
        }
    }
}
