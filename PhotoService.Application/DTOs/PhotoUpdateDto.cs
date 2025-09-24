
namespace PhotoService.Application.DTOs
{
    public class PhotoUpdateDto
    {
        public Guid PhotoGuid { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Country { get; set; }

        public DateTime? DateTaken { get; set; }
    }
}
