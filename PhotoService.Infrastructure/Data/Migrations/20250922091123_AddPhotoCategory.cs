using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhotoCategories",
                columns: table => new
                {
                    PhotoGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoCategories", x => new { x.PhotoGuid, x.CategoryGuid });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhotoCategories");
        }
    }
}
