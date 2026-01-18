using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_00007 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "fk_reservation_facility_facility_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_plan_plan_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_room_group_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_site_site_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_user_user_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropIndex(
                name: "ix_reservation_user_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.AlterColumn<long>(
                name: "site_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "room_group_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "reserver_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "plan_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "main_user_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "facility_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_customer_info_main_user_id",
                schema: "public",
                table: "reservation",
                column: "main_user_id",
                principalSchema: "public",
                principalTable: "customer_info",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_customer_info_reserver_id",
                schema: "public",
                table: "reservation",
                column: "reserver_id",
                principalSchema: "public",
                principalTable: "customer_info",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_facility_facility_id",
                schema: "public",
                table: "reservation",
                column: "facility_id",
                principalSchema: "public",
                principalTable: "facility",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_plan_plan_id",
                schema: "public",
                table: "reservation",
                column: "plan_id",
                principalSchema: "public",
                principalTable: "plan",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_room_group_room_group_id",
                schema: "public",
                table: "reservation",
                column: "room_group_id",
                principalSchema: "public",
                principalTable: "room_group",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_site_site_id",
                schema: "public",
                table: "reservation",
                column: "site_id",
                principalSchema: "public",
                principalTable: "site",
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
                name: "fk_reservation_facility_facility_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_plan_plan_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_room_group_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_site_site_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.AlterColumn<long>(
                name: "site_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "room_group_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "reserver_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "plan_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "main_user_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "facility_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_user_id",
                schema: "public",
                table: "reservation",
                column: "user_id");

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
                name: "fk_reservation_facility_facility_id",
                schema: "public",
                table: "reservation",
                column: "facility_id",
                principalSchema: "public",
                principalTable: "facility",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_plan_plan_id",
                schema: "public",
                table: "reservation",
                column: "plan_id",
                principalSchema: "public",
                principalTable: "plan",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_room_group_room_group_id",
                schema: "public",
                table: "reservation",
                column: "room_group_id",
                principalSchema: "public",
                principalTable: "room_group",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_site_site_id",
                schema: "public",
                table: "reservation",
                column: "site_id",
                principalSchema: "public",
                principalTable: "site",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_user_user_id",
                schema: "public",
                table: "reservation",
                column: "user_id",
                principalSchema: "public",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
