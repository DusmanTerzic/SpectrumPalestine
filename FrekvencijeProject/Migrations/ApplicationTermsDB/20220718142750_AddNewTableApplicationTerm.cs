using Microsoft.EntityFrameworkCore.Migrations;

namespace FrekvencijeProject.Migrations.ApplicationTermsDB
{
    public partial class AddNewTableApplicationTerm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RootApplicationTermsDB");
        }
    }
}
