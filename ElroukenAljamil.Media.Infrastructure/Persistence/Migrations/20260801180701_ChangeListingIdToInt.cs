using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElroukenAljamil.Media.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeListingIdToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_ListingId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "ListingId",
                table: "MediaFiles");

            migrationBuilder.AddColumn<int>(
                name: "ListingId",
                table: "MediaFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_ListingId",
                table: "MediaFiles",
                column: "ListingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ListingId",
                table: "MediaFiles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
