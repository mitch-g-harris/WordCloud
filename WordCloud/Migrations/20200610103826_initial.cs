using Microsoft.EntityFrameworkCore.Migrations;

namespace WordCloud.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteWords",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Word = table.Column<string>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteWords", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteWords");
        }
    }
}
