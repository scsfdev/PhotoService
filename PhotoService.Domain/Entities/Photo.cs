
namespace PhotoService.Domain.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public Guid PhotoGuid { get; set; } = Guid.NewGuid();

        public required string FileName { get; set; }
        
        // Remove this local path demo and switch to GCS bucket to simulate real world situation.
        //public required string FilePath { get; set; }

        // Since all will be image, no need to separately keep MimeType.
        //public required string MimeType { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Country { get; set; }

        public DateTime? DateTaken { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int LikesCount { get; set; } = 0;

        // Navigation properties
        public ICollection<PhotoLike> PhotoLikes { get; set; } = new List<PhotoLike>();

        public ICollection<PhotoCategory> PhotoCategories { get; set; } = new List<PhotoCategory>();
    }
}
