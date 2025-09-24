
namespace PhotoService.Application.DTOs
{
    public class PhotoDto
    {
        public Guid PhotoGuid { get; set; } 

        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public required string MimeType { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Country { get; set; }

        public DateTime? DateTaken { get; set; }
        public DateTime UploadedAt { get; set; } 

        public int LikesCount { get; set; }

        public ICollection<PhotoLikeDto> PhotoLikes { get; set; } = new List<PhotoLikeDto>();

        public ICollection<PhotoCategoryDto> PhotoCategories { get; set; } = new List<PhotoCategoryDto>();
    }
}
