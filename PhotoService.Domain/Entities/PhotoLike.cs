namespace PhotoService.Domain.Entities
{
    public class PhotoLike
    {
        public int Id { get; set; }
        
        public Guid PhotoGuid { get; set; }
        public Guid UserGuid { get; set; }

        public DateTime LikedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Photo? Photo { get; set; }
    }
}