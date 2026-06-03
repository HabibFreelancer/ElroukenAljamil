using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElroukenAljamil.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceAnnonceCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Annonces");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Annonces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Annonces_CategoryId",
                table: "Annonces",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Annonces_Categories_CategoryId",
                table: "Annonces",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Annonces_Categories_CategoryId",
                table: "Annonces");

            migrationBuilder.DropIndex(
                name: "IX_Annonces_CategoryId",
                table: "Annonces");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Annonces");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Annonces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
