using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_00004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservation_user_info_main_user_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_user_info_reserver_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_plan_room_group_app_date_user_info_user_info_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropForeignKey(
                name: "fk_user_user_info_user_info_id",
                schema: "public",
                table: "user");

            migrationBuilder.DropTable(
                name: "user_info",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "customer_info",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    kana = table.Column<string>(type: "text", nullable: true),
                    e_mail = table.Column<string>(type: "text", nullable: true),
                    name_e = table.Column<string>(type: "text", nullable: true),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    country_id = table.Column<long>(type: "bigint", nullable: true),
                    post_code = table.Column<string>(type: "text", nullable: true),
                    address1 = table.Column<string>(type: "text", nullable: true),
                    address2 = table.Column<string>(type: "text", nullable: true),
                    address3 = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    birth_day = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("pk_customer_info", x => x.id);
                    table.ForeignKey(
                        name: "fk_customer_info_country_country_id",
                        column: x => x.country_id,
                        principalSchema: "public",
                        principalTable: "country",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_customer_info_code",
                schema: "public",
                table: "customer_info",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_customer_info_country_id",
                schema: "public",
                table: "customer_info",
                column: "country_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_customer_info_main_user_id",
                schema: "public",
                table: "reservation",
                column: "main_user_id",
                principalSchema: "public",
                principalTable: "customer_info",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_customer_info_reserver_id",
                schema: "public",
                table: "reservation",
                column: "reserver_id",
                principalSchema: "public",
                principalTable: "customer_info",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_plan_room_group_app_date_customer_info_user_inf",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                column: "user_info_id",
                principalSchema: "public",
                principalTable: "customer_info",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_customer_info_user_info_id",
                schema: "public",
                table: "user",
                column: "user_info_id",
                principalSchema: "public",
                principalTable: "customer_info",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservation_customer_info_main_user_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_customer_info_reserver_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_plan_room_group_app_date_customer_info_user_inf",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropForeignKey(
                name: "fk_user_customer_info_user_info_id",
                schema: "public",
                table: "user");

            migrationBuilder.DropTable(
                name: "customer_info",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "user_info",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    country_id = table.Column<long>(type: "bigint", nullable: true),
                    address1 = table.Column<string>(type: "text", nullable: true),
                    address2 = table.Column<string>(type: "text", nullable: true),
                    address3 = table.Column<string>(type: "text", nullable: true),
                    birth_day = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    e_mail = table.Column<string>(type: "text", nullable: true),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    kana = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    name_e = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    post_code = table.Column<string>(type: "text", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_info", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_info_country_country_id",
                        column: x => x.country_id,
                        principalSchema: "public",
                        principalTable: "country",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_info_code",
                schema: "public",
                table: "user_info",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_info_country_id",
                schema: "public",
                table: "user_info",
                column: "country_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_user_info_main_user_id",
                schema: "public",
                table: "reservation",
                column: "main_user_id",
                principalSchema: "public",
                principalTable: "user_info",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_user_info_reserver_id",
                schema: "public",
                table: "reservation",
                column: "reserver_id",
                principalSchema: "public",
                principalTable: "user_info",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_plan_room_group_app_date_user_info_user_info_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                column: "user_info_id",
                principalSchema: "public",
                principalTable: "user_info",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_user_info_user_info_id",
                schema: "public",
                table: "user",
                column: "user_info_id",
                principalSchema: "public",
                principalTable: "user_info",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
