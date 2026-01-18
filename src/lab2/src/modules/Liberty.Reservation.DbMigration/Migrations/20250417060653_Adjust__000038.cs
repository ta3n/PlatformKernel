using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust__000038 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
                -- @formatter:off
                UPDATE public.room_group
                SET is_description_visible = true
                WHERE is_description_visible = false;
                -- @formatter:on
            ");
            migrationBuilder.Sql(@$"
                -- @formatter:off
                UPDATE public.room_group
                SET is_overview_visible = true
                WHERE is_overview_visible = false;
                -- @formatter:off
            ");

            migrationBuilder.AlterColumn<bool>(
                name: "is_overview_visible",
                schema: "public",
                table: "room_group",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_description_visible",
                schema: "public",
                table: "room_group",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_overview_visible",
                schema: "public",
                table: "room_group",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_description_visible",
                schema: "public",
                table: "room_group",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);
        }
    }
}
