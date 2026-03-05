using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address1",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "address2",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "address3",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "kana",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "name",
                schema: "public",
                table: "facility");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address1",
                schema: "public",
                table: "facility",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address2",
                schema: "public",
                table: "facility",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address3",
                schema: "public",
                table: "facility",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kana",
                schema: "public",
                table: "facility",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                schema: "public",
                table: "facility",
                type: "text",
                nullable: true);
        }
    }
}
