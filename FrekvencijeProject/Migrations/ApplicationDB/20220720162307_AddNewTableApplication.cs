using Microsoft.EntityFrameworkCore.Migrations;

namespace FrekvencijeProject.Migrations.ApplicationDB
{
    public partial class AddNewTableApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RootApplicationDB",
                columns: table => new
                {
                    RootApplicationDBId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    regionId = table.Column<int>(nullable: false),
                    regionName = table.Column<string>(nullable: true),
                    regionCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RootApplicationDB", x => x.RootApplicationDBId);
                });

            migrationBuilder.CreateTable(
                name: "RootApplicationTermsDB",
                columns: table => new
                {
                    ApplicationTermsDBId = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RootApplicationTermsDB", x => x.ApplicationTermsDBId);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationRange",
                columns: table => new
                {
                    ApplicationRangeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    low = table.Column<long>(nullable: false),
                    high = table.Column<long>(nullable: false),
                    RootApplicationDBId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRange", x => x.ApplicationRangeId);
                    table.ForeignKey(
                        name: "FK_ApplicationRange_RootApplicationDB_RootApplicationDBId",
                        column: x => x.RootApplicationDBId,
                        principalTable: "RootApplicationDB",
                        principalColumn: "RootApplicationDBId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationRangeId = table.Column<int>(nullable: true),
                    ApplicationTermId = table.Column<int>(nullable: true),
                    comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_Application_ApplicationRange_ApplicationRangeId",
                        column: x => x.ApplicationRangeId,
                        principalTable: "ApplicationRange",
                        principalColumn: "ApplicationRangeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Application_RootApplicationTermsDB_ApplicationTermId",
                        column: x => x.ApplicationTermId,
                        principalTable: "RootApplicationTermsDB",
                        principalColumn: "ApplicationTermsDBId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application_ApplicationRangeId",
                table: "Application",
                column: "ApplicationRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Application_ApplicationTermId",
                table: "Application",
                column: "ApplicationTermId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRange_RootApplicationDBId",
                table: "ApplicationRange",
                column: "RootApplicationDBId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "ApplicationRange");

            migrationBuilder.DropTable(
                name: "RootApplicationTermsDB");

            migrationBuilder.DropTable(
                name: "RootApplicationDB");
        }
    }
}
