using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "author",
                table: "posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "author",
                table: "posts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
