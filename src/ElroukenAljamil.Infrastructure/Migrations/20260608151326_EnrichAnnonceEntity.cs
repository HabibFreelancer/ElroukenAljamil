using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElroukenAljamil.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnrichAnnonceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdType",
                table: "Annonces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "Annonces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Annonces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraData",
                table: "Annonces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "HidePhone",
                table: "Annonces",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Annonces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Annonces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdType",
                table: "Annonces");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "Annonces");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Annonces");

            migrationBuilder.DropColumn(
                name: "ExtraData",
                table: "Annonces");

            migrationBuilder.DropColumn(
                name: "HidePhone",
                table: "Annonces");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Annonces");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Annonces");
        }
    }
}
