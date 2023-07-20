using Microsoft.EntityFrameworkCore.Migrations;

namespace FrekvencijeProject.Migrations.AllocationDB
{
    public partial class AddNewTablesAllocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllocationTermDb",
                columns: table => new
                {
                    AllocationTermId = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationTermDb", x => x.AllocationTermId);
                });

            migrationBuilder.CreateTable(
                name: "RootAllocationDB",
                columns: table => new
                {
                    RootAllocationDBId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    regionId = table.Column<int>(nullable: false),
                    regionName = table.Column<string>(nullable: true),
                    regionCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RootAllocationDB", x => x.RootAllocationDBId);
                });

            migrationBuilder.CreateTable(
                name: "AllocationRangeDb",
                columns: table => new
                {
                    AllocationRangeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    low = table.Column<long>(nullable: false),
                    high = table.Column<long>(nullable: false),
                    RootAllocationDBId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationRangeDb", x => x.AllocationRangeId);
                    table.ForeignKey(
                        name: "FK_AllocationRangeDb_RootAllocationDB_RootAllocationDBId",
                        column: x => x.RootAllocationDBId,
                        principalTable: "RootAllocationDB",
                        principalColumn: "RootAllocationDBId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AllocationDb",
                columns: table => new
                {
                    AllocationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllocationTermId = table.Column<int>(nullable: true),
                    primary = table.Column<bool>(nullable: false),
                    AllocationRangeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationDb", x => x.AllocationId);
                    table.ForeignKey(
                        name: "FK_AllocationDb_AllocationRangeDb_AllocationRangeId",
                        column: x => x.AllocationRangeId,
                        principalTable: "AllocationRangeDb",
                        principalColumn: "AllocationRangeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllocationDb_AllocationTermDb_AllocationTermId",
                        column: x => x.AllocationTermId,
                        principalTable: "AllocationTermDb",
                        principalColumn: "AllocationTermId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FootnoteAllocation",
                columns: table => new
                {
                    FootnoteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    AllocationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FootnoteAllocation", x => x.FootnoteId);
                    table.ForeignKey(
                        name: "FK_FootnoteAllocation_AllocationDb_AllocationId",
                        column: x => x.AllocationId,
                        principalTable: "AllocationDb",
                        principalColumn: "AllocationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllocationDb_AllocationRangeId",
                table: "AllocationDb",
                column: "AllocationRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocationDb_AllocationTermId",
                table: "AllocationDb",
                column: "AllocationTermId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocationRangeDb_RootAllocationDBId",
                table: "AllocationRangeDb",
                column: "RootAllocationDBId");

            migrationBuilder.CreateIndex(
                name: "IX_FootnoteAllocation_AllocationId",
                table: "FootnoteAllocation",
                column: "AllocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FootnoteAllocation");

            migrationBuilder.DropTable(
                name: "AllocationDb");

            migrationBuilder.DropTable(
                name: "AllocationRangeDb");

            migrationBuilder.DropTable(
                name: "AllocationTermDb");

            migrationBuilder.DropTable(
                name: "RootAllocationDB");
        }
    }
}
