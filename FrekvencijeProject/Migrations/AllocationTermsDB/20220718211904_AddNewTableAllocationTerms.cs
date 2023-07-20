using Microsoft.EntityFrameworkCore.Migrations;

namespace FrekvencijeProject.Migrations.AllocationTermsDB
{
    public partial class AddNewTableAllocationTerms : Migration
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllocationTermDb");
        }
    }
}
