using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_00006 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_gmo_payment_result_request_reservation_reservation_id",
                schema: "public",
                table: "order_gmo_payment_result_request");

            migrationBuilder.DropIndex(
                name: "ix_order_gmo_payment_result_request_reservation_id",
                schema: "public",
                table: "order_gmo_payment_result_request");

            migrationBuilder.DropColumn(
                name: "enabled_date_end",
                schema: "public",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "enabled_date_start",
                schema: "public",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "reservation_id",
                schema: "public",
                table: "order_gmo_payment_result_request");

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
                oldDefaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserId\":0,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"UserDatas\":null,\"PersonAgeTypes\":null,\"ReservationPersonDatas\":null,\"ReservationPriceDatas\":null,\"ReservationOptionItems\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0,\"TotalDiscount\":0,\"AllTotalPrice\":0,\"HasOptionItem\":false}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "reservation_data",
                schema: "public",
                table: "reservation",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserId\":0,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"UserDatas\":null,\"PersonAgeTypes\":null,\"ReservationPersonDatas\":null,\"ReservationPriceDatas\":null,\"ReservationOptionItems\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0,\"TotalDiscount\":0,\"AllTotalPrice\":0,\"HasOptionItem\":false}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserId\":0,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0,\"TotalDiscount\":0,\"AllTotalPrice\":0,\"HasOptionItem\":false}");

            migrationBuilder.AddColumn<long>(
                name: "enabled_date_end",
                schema: "public",
                table: "plan",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "enabled_date_start",
                schema: "public",
                table: "plan",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "reservation_id",
                schema: "public",
                table: "order_gmo_payment_result_request",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_order_gmo_payment_result_request_reservation_id",
                schema: "public",
                table: "order_gmo_payment_result_request",
                column: "reservation_id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_gmo_payment_result_request_reservation_reservation_id",
                schema: "public",
                table: "order_gmo_payment_result_request",
                column: "reservation_id",
                principalSchema: "public",
                principalTable: "reservation",
                principalColumn: "id");
        }
    }
}
