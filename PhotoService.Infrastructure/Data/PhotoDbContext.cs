using Microsoft.EntityFrameworkCore;
using PhotoService.Domain.Entities;

namespace PhotoService.Infrastructure.Data
{
    public class PhotoDbContext(DbContextOptions<PhotoDbContext> options) : DbContext(options)
    {
        // Define DbSets for entities
        public DbSet<Photo> Photos { get; set; } = null!;
        public DbSet<PhotoLike> PhotoLikes { get; set; } = null!;
        public DbSet<PhotoCategory> PhotoCategories { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.Property(e => e.PhotoGuid).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.UploadedAt).HasDefaultValueSql("GETDATE()").HasColumnType("datetime2(0)");
                entity.Property(e => e.LikesCount).HasDefaultValue(0);

                entity.HasMany(e => e.PhotoLikes)
                      .WithOne(p1 => p1.Photo)
                      .HasForeignKey(p1 => p1.PhotoGuid)
                      .HasPrincipalKey(p => p.PhotoGuid)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PhotoLike>(entity =>
            {
                entity.Property(e => e.LikedAt).HasDefaultValueSql("GETDATE()").HasColumnType("datetime2(0)");
            });

            modelBuilder.Entity<PhotoCategory>(entity =>
            {
                entity.HasKey(pc => new { pc.PhotoGuid, pc.CategoryGuid });
                entity.Property(pc => pc.PhotoGuid).IsRequired();
                entity.Property(pc => pc.CategoryGuid).IsRequired();

                entity.HasOne<Photo>()
                      .WithMany(p => p.PhotoCategories)
                      .HasForeignKey(pc => pc.PhotoGuid)
                      .HasPrincipalKey(p => p.PhotoGuid)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
