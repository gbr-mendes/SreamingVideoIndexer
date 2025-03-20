using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamingVideoIndexer.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnThumbnailRemoteKeyToTableIndexedFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailRemoteKey",
                table: "IndexedFiles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailRemoteKey",
                table: "IndexedFiles");
        }
    }
}
