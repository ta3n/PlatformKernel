using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000031_AddMultilingualSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_plan_meta",
                schema: "public",
                table: "plan",
                column: "meta")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_facility_meta",
                schema: "public",
                table: "facility",
                column: "meta")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_plan_meta",
                schema: "public",
                table: "plan");

            migrationBuilder.DropIndex(
                name: "ix_facility_meta",
                schema: "public",
                table: "facility");
        }
    }
}
