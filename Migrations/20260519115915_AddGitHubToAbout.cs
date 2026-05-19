using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioCMS.Migrations
{
    /// <inheritdoc />
    public partial class AddGitHubToAbout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MediumValue",
                table: "AboutContent",
                newName: "GithubLink");

            migrationBuilder.RenameColumn(
                name: "MediumLabel",
                table: "AboutContent",
                newName: "Github");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GithubLink",
                table: "AboutContent",
                newName: "MediumValue");

            migrationBuilder.RenameColumn(
                name: "Github",
                table: "AboutContent",
                newName: "MediumLabel");
        }
    }
}
