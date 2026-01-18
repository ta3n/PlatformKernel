using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000070_AddPlanRoomGroupSitePriceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "plan_room_group_site_price_data",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    person_min = table.Column<int>(type: "integer", nullable: false),
                    person_max = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<int>(type: "integer", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_room_group_site_price_data", x => new { x.plan_id, x.room_group_id, x.site_id, x.person_min, x.person_max });
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_price_data_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_price_data_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_price_data_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_price_data_room_group_id",
                schema: "public",
                table: "plan_room_group_site_price_data",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_price_data_site_id",
                schema: "public",
                table: "plan_room_group_site_price_data",
                column: "site_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "plan_room_group_site_price_data",
                schema: "public");
        }
    }
}
