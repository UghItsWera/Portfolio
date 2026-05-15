using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioCMS.Migrations
{
    /// <inheritdoc />
    public partial class AddAboutContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutContent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProfileImage = table.Column<string>(type: "TEXT", nullable: true),
                    Bio1 = table.Column<string>(type: "TEXT", nullable: true),
                    Bio2 = table.Column<string>(type: "TEXT", nullable: true),
                    Bio3 = table.Column<string>(type: "TEXT", nullable: true),
                    LinkedIn = table.Column<string>(type: "TEXT", nullable: true),
                    LinkedInLink = table.Column<string>(type: "TEXT", nullable: true),
                    CV = table.Column<string>(type: "TEXT", nullable: true),
                    CVDownload = table.Column<string>(type: "TEXT", nullable: true),
                    MediumLabel = table.Column<string>(type: "TEXT", nullable: true),
                    MediumValue = table.Column<string>(type: "TEXT", nullable: true),
                    EmailAddress = table.Column<string>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutContent", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutContent");
        }
    }
}
