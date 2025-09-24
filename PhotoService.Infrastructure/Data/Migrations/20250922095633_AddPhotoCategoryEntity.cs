using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoCategoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_PhotoCategories_Photos_PhotoGuid",
                table: "PhotoCategories",
                column: "PhotoGuid",
                principalTable: "Photos",
                principalColumn: "PhotoGuid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoCategories_Photos_PhotoGuid",
                table: "PhotoCategories");
        }
    }
}
