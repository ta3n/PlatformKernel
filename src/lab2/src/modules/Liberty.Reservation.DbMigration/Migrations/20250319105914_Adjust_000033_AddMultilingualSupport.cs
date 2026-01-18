using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000033_AddMultilingualSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "overview",
                schema: "public",
                table: "room_group",
                type: "jsonb",
                nullable: true,
                defaultValueSql: "'{}'::jsonb");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_overview",
                schema: "public",
                table: "room_group",
                column: "overview")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_room_group_overview",
                schema: "public",
                table: "room_group");

            migrationBuilder.DropColumn(
                name: "overview",
                schema: "public",
                table: "room_group");
        }
    }
}
