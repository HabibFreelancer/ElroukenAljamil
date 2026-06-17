using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElroukenAljamil.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVisibilityCondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VisibilityCondition",
                table: "StepFields",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisibilityCondition",
                table: "StepFields");
        }
    }
}
