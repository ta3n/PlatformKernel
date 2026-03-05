using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000013 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_application_user_favorite_user_user_id",
                schema: "public",
                table: "application_user_favorite");

            migrationBuilder.DropForeignKey(
                name: "fk_application_user_point_user_user_id",
                schema: "public",
                table: "application_user_point");

            migrationBuilder.DropTable(
                name: "application_user_login_history",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_application_user",
                schema: "public");

            migrationBuilder.DropTable(
                name: "login_history",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user",
                schema: "public");

            migrationBuilder.DropPrimaryKey(
                name: "pk_application_user_point",
                schema: "public",
                table: "application_user_point");

            migrationBuilder.DropPrimaryKey(
                name: "pk_application_user_favorite",
                schema: "public",
                table: "application_user_favorite");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "public",
                table: "application_user_point");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "public",
                table: "application_user_favorite");

            migrationBuilder.AddColumn<string>(
                name: "user_code",
                schema: "public",
                table: "application_user_point",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_code",
                schema: "public",
                table: "application_user_favorite",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_application_user_point",
                schema: "public",
                table: "application_user_point",
                columns: new[] { "user_code", "point_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_application_user_favorite",
                schema: "public",
                table: "application_user_favorite",
                columns: new[] { "user_code", "favorite_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_application_user_point",
                schema: "public",
                table: "application_user_point");

            migrationBuilder.DropPrimaryKey(
                name: "pk_application_user_favorite",
                schema: "public",
                table: "application_user_favorite");

            migrationBuilder.DropColumn(
                name: "user_code",
                schema: "public",
                table: "application_user_point");

            migrationBuilder.DropColumn(
                name: "user_code",
                schema: "public",
                table: "application_user_favorite");

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                schema: "public",
                table: "application_user_point",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                schema: "public",
                table: "application_user_favorite",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "pk_application_user_point",
                schema: "public",
                table: "application_user_point",
                columns: new[] { "user_id", "point_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_application_user_favorite",
                schema: "public",
                table: "application_user_favorite",
                columns: new[] { "user_id", "favorite_id" });

            migrationBuilder.CreateTable(
                name: "login_history",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    meta_json = table.Column<string>(type: "text", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_login_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", maxLength: 36, nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_info_id = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    memo = table.Column<string>(type: "text", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    user_info_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_customer_info_customer_info_id",
                        column: x => x.customer_info_id,
                        principalSchema: "public",
                        principalTable: "customer_info",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "application_user_login_history",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    login_history_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_user_login_history", x => new { x.user_id, x.login_history_id });
                    table.ForeignKey(
                        name: "fk_application_user_login_history_login_history_login_history_",
                        column: x => x.login_history_id,
                        principalSchema: "public",
                        principalTable: "login_history",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_application_user_login_history_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_application_user",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_application_user", x => new { x.facility_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_facility_application_user_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_application_user_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_application_user_login_history_login_history_id",
                schema: "public",
                table: "application_user_login_history",
                column: "login_history_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_application_user_user_id",
                schema: "public",
                table: "facility_application_user",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_login_history_code",
                schema: "public",
                table: "login_history",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_code",
                schema: "public",
                table: "user",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_customer_info_id",
                schema: "public",
                table: "user",
                column: "customer_info_id");

            migrationBuilder.AddForeignKey(
                name: "fk_application_user_favorite_user_user_id",
                schema: "public",
                table: "application_user_favorite",
                column: "user_id",
                principalSchema: "public",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_application_user_point_user_user_id",
                schema: "public",
                table: "application_user_point",
                column: "user_id",
                principalSchema: "public",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
