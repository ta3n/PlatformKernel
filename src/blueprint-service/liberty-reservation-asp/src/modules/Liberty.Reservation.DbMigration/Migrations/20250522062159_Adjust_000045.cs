using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000045 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_file_room_group",
                schema: "public",
                table: "file_room_group");

            migrationBuilder.DropPrimaryKey(
                name: "pk_file_plan",
                schema: "public",
                table: "file_plan");

            migrationBuilder.DropPrimaryKey(
                name: "pk_file_option_item",
                schema: "public",
                table: "file_option_item");

            migrationBuilder.AddPrimaryKey(
                name: "pk_file_room_group",
                schema: "public",
                table: "file_room_group",
                columns: new[] { "file_id", "room_group_id", "index" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_file_plan",
                schema: "public",
                table: "file_plan",
                columns: new[] { "file_id", "plan_id", "index" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_file_option_item",
                schema: "public",
                table: "file_option_item",
                columns: new[] { "file_id", "option_item_id", "index" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_file_room_group",
                schema: "public",
                table: "file_room_group");

            migrationBuilder.DropPrimaryKey(
                name: "pk_file_plan",
                schema: "public",
                table: "file_plan");

            migrationBuilder.DropPrimaryKey(
                name: "pk_file_option_item",
                schema: "public",
                table: "file_option_item");

            migrationBuilder.AddPrimaryKey(
                name: "pk_file_room_group",
                schema: "public",
                table: "file_room_group",
                columns: new[] { "file_id", "room_group_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_file_plan",
                schema: "public",
                table: "file_plan",
                columns: new[] { "file_id", "plan_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_file_option_item",
                schema: "public",
                table: "file_option_item",
                columns: new[] { "file_id", "option_item_id" });
        }
    }
}
