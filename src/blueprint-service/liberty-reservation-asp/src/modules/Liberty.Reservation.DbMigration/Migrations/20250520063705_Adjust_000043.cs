using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000043 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_no_show",
                schema: "public",
                table: "reservation",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_no_show",
                schema: "public",
                table: "reservation");
        }
    }
}
