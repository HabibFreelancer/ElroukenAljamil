using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElroukenAljamil.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Annonces_CategoryId",
                table: "Annonces");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Annonces",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Annonces_CategoryId_Title",
                table: "Annonces",
                columns: new[] { "CategoryId", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_Annonces_Title",
                table: "Annonces",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Annonces_CategoryId_Title",
                table: "Annonces");

            migrationBuilder.DropIndex(
                name: "IX_Annonces_Title",
                table: "Annonces");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Annonces",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Annonces_CategoryId",
                table: "Annonces",
                column: "CategoryId");
        }
    }
}
