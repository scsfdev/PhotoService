
namespace PhotoService.Application.DTOs
{
    public class PhotoLikeDto
    {
        public Guid PhotoGuid { get; set; }
        public Guid UserGuid { get; set; }

        public DateTime LikedAt { get; set; }
    }
}
