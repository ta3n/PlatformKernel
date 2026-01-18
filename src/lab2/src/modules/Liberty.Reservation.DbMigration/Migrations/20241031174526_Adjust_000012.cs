using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000012 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_facility_country_country_id",
                schema: "public",
                table: "facility");

            migrationBuilder.DropIndex(
                name: "ix_facility_country_id",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "country_id",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "post_code",
                schema: "public",
                table: "facility");

            migrationBuilder.AlterColumn<string>(
                name: "reservation_data",
                schema: "public",
                table: "reservation",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserCode\":null,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0.0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"HasOptionItem\":false,\"SendMailState\":{\"BookingReminderOfUpcomingCheckInDateSend\":null,\"BookingReminderOfUpcomingCheckInDateSent\":null,\"BookingCancellationFeeReminderSend\":null,\"BookingCancellationFeeReminderSent\":null}}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserCode\":null,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0.0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"HasOptionItem\":false}");
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
                defaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserCode\":null,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0.0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"HasOptionItem\":false}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserCode\":null,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0.0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"HasOptionItem\":false,\"SendMailState\":{\"BookingReminderOfUpcomingCheckInDateSend\":null,\"BookingReminderOfUpcomingCheckInDateSent\":null,\"BookingCancellationFeeReminderSend\":null,\"BookingCancellationFeeReminderSent\":null}}");

            migrationBuilder.AddColumn<long>(
                name: "country_id",
                schema: "public",
                table: "facility",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "post_code",
                schema: "public",
                table: "facility",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_facility_country_id",
                schema: "public",
                table: "facility",
                column: "country_id");

            migrationBuilder.AddForeignKey(
                name: "fk_facility_country_country_id",
                schema: "public",
                table: "facility",
                column: "country_id",
                principalSchema: "public",
                principalTable: "country",
                principalColumn: "id");
        }
    }
}
