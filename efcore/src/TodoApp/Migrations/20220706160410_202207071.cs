using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Migrations
{
    public partial class _202207071 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TodoItem",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                comment: "姓名")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TodoTest",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Test = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false, comment: "测试")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoTest", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoTest");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TodoItem");
        }
    }
}
