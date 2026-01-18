using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000056_V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "cancel_rate_fee",
                schema: "public",
                table: "reservation",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rule_detail",
                schema: "public",
                table: "cancellation",
                type: "jsonb",
                nullable: true,
                defaultValueSql: "'{}'::jsonb");

            migrationBuilder.CreateIndex(
                name: "ix_cancellation_rule_detail",
                schema: "public",
                table: "cancellation",
                column: "rule_detail")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_cancellation_rule_detail",
                schema: "public",
                table: "cancellation");

            migrationBuilder.DropColumn(
                name: "cancel_rate_fee",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "rule_detail",
                schema: "public",
                table: "cancellation");
        }
    }
}
