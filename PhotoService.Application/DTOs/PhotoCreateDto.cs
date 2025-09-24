
namespace PhotoService.Application.DTOs
{
    public class PhotoCreateDto
    {
        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public required string MimeType { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Country { get; set; }

        public DateTime? DateTaken { get; set; }
    }
}
