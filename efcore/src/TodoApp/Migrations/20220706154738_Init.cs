using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TodoItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Text = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false, comment: "事情")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItem", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoItem");
        }
    }
}
