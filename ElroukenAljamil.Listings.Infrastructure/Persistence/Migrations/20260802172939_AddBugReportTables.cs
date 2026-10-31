using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElroukenAljamil.Listings.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBugReportTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BugReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Module = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Environment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StepsToReproduce = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExpectedBehavior = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ActualBehavior = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ScreenshotUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BrowserInfo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReportedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReportedByUserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BugComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BugReportId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugComments_BugReports_BugReportId",
                        column: x => x.BugReportId,
                        principalTable: "BugReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BugComments_BugReportId",
                table: "BugComments",
                column: "BugReportId");

            migrationBuilder.CreateIndex(
                name: "IX_BugComments_CreatedAt",
                table: "BugComments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_CreatedAt",
                table: "BugReports",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_ReportedByUserId",
                table: "BugReports",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_Severity",
                table: "BugReports",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_BugReports_Status",
                table: "BugReports",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BugComments");

            migrationBuilder.DropTable(
                name: "BugReports");
        }
    }
}
