using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_00008 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_option_item_option_item_app",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropIndex(
                name: "ix_reservation_room_group_app_date_option_item_option_item_id_",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropColumn(
                name: "meta_json",
                schema: "public",
                table: "cancellation");

            migrationBuilder.AlterColumn<decimal>(
                name: "unit_price",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "spa_tax",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "price",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<long>(
                name: "option_item_app_date_app_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "option_item_app_date_option_item_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reservation_data",
                schema: "public",
                table: "reservation",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserId\":0,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0.0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"HasOptionItem\":false}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserId\":0,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0,\"TotalDiscount\":0,\"AllTotalPrice\":0,\"HasOptionItem\":false}");

            migrationBuilder.AlterColumn<decimal>(
                name: "cancellation_price",
                schema: "public",
                table: "reservation",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "meta",
                schema: "public",
                table: "cancellation",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"TableSource\":null}");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_group_app_date_option_item_option_item_app",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                columns: new[] { "option_item_app_date_option_item_id", "option_item_app_date_app_date_id" });

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_group_app_date_option_item_option_item_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                column: "option_item_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_room_group_app_date_option_item_option_item_app",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                columns: new[] { "option_item_app_date_option_item_id", "option_item_app_date_app_date_id" },
                principalSchema: "public",
                principalTable: "option_item_app_date",
                principalColumns: new[] { "option_item_id", "app_date_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_option_item_option_item_app",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropIndex(
                name: "ix_reservation_room_group_app_date_option_item_option_item_app",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropIndex(
                name: "ix_reservation_room_group_app_date_option_item_option_item_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropColumn(
                name: "option_item_app_date_app_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropColumn(
                name: "option_item_app_date_option_item_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropColumn(
                name: "meta",
                schema: "public",
                table: "cancellation");

            migrationBuilder.AlterColumn<int>(
                name: "unit_price",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "spa_tax",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "price",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "reservation_data",
                schema: "public",
                table: "reservation",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserId\":0,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0,\"TotalDiscount\":0,\"AllTotalPrice\":0,\"HasOptionItem\":false}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserId\":0,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0.0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"HasOptionItem\":false}");

            migrationBuilder.AlterColumn<int>(
                name: "cancellation_price",
                schema: "public",
                table: "reservation",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "meta_json",
                schema: "public",
                table: "cancellation",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_group_app_date_option_item_option_item_id_",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                columns: new[] { "option_item_id", "app_date_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_room_group_app_date_option_item_option_item_app",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                columns: new[] { "option_item_id", "app_date_id" },
                principalSchema: "public",
                principalTable: "option_item_app_date",
                principalColumns: new[] { "option_item_id", "app_date_id" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
