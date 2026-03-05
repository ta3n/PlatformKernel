using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000055_AddGmoReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gmo_change_tran_report",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<string>(type: "text", nullable: true),
                    reservation_id = table.Column<long>(type: "bigint", nullable: false),
                    access_id = table.Column<string>(type: "text", nullable: true),
                    request = table.Column<string>(type: "jsonb", nullable: true),
                    response = table.Column<string>(type: "jsonb", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gmo_change_tran_report", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_gmo_change_tran_report_code",
                schema: "public",
                table: "gmo_change_tran_report",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gmo_change_tran_report",
                schema: "public");
        }
    }
}
