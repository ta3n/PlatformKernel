using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000047 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "md5",
                schema: "public",
                table: "file",
                newName: "encrypt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "encrypt",
                schema: "public",
                table: "file",
                newName: "md5");
        }
    }
}
