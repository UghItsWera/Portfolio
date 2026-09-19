using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioCMS.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryThemes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryThemes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    LightBackground = table.Column<string>(type: "TEXT", nullable: false),
                    LightAccent = table.Column<string>(type: "TEXT", nullable: false),
                    LightCard = table.Column<string>(type: "TEXT", nullable: false),
                    DarkBackground = table.Column<string>(type: "TEXT", nullable: false),
                    DarkAccent = table.Column<string>(type: "TEXT", nullable: false),
                    DarkCard = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryThemes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryThemes_Category",
                table: "CategoryThemes",
                column: "Category",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryThemes");
        }
    }
}
