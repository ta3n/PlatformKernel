using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000068_AddNoticeNumberInReservationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "booking_data",
                schema: "public",
                table: "reservation",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"LanguageCode\":null,\"Facility\":{\"Id\":null,\"Name\":null,\"Address\":\"\",\"FullAddress\":\"\",\"Address1\":null,\"Address2\":null,\"Address3\":null,\"Address4\":null,\"Tel\":null,\"CanOnLinePayment\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null},\"Plan\":{\"Id\":null,\"Name\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null,\"DayUse\":false,\"Meta\":null,\"Meals\":null,\"Files\":null,\"IsCancelSameAccept\":false,\"CancelDayLimit\":null,\"ReceptionDayLimit\":null,\"CancelLimit\":null,\"CancellationDataPolicy\":null},\"RoomGroup\":{\"Id\":null,\"Name\":null,\"GroupName\":null,\"IsEnabledSmoking\":null,\"CapacityMax\":null,\"CapacityMin\":null,\"Files\":null},\"Site\":{\"Id\":null,\"Name\":null,\"ShortName\":null,\"PrefixName\":null},\"PlanQuestions\":null,\"OptionQuestions\":null,\"AppDates\":null,\"PersonAgeTypes\":null,\"SendMailState\":{\"BookingReminderOfUpcomingCheckInDateSend\":null,\"BookingReminderOfUpcomingCheckInDateSent\":null,\"BookingCancellationFeeReminderSend\":null,\"BookingCancellationFeeReminderSent\":null},\"TotalRoomPrice\":0.0,\"TotalSpaTax\":0.0,\"TotalOptionPrice\":0.0,\"UsedPoint\":0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"TotalPrice\":0.0,\"IsSiteLocation\":false,\"TimeZoneOffset\":0,\"MediaCode\":null,\"RootCode\":null}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"LanguageCode\":null,\"Facility\":{\"Id\":null,\"Name\":null,\"Address\":\"\",\"FullAddress\":\"\",\"Address1\":null,\"Address2\":null,\"Address3\":null,\"Address4\":null,\"Tel\":null,\"CanOnLinePayment\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null},\"Plan\":{\"Id\":null,\"Name\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null,\"DayUse\":false,\"Meta\":null,\"Meals\":null,\"Files\":null,\"IsCancelSameAccept\":false,\"CancelDayLimit\":null,\"ReceptionDayLimit\":null,\"CancelLimit\":null,\"CancellationDataPolicy\":null},\"RoomGroup\":{\"Id\":null,\"Name\":null,\"GroupName\":null,\"IsEnabledSmoking\":null,\"CapacityMax\":null,\"CapacityMin\":null,\"Files\":null},\"Site\":{\"Id\":null,\"Name\":null,\"ShortName\":null},\"PlanQuestions\":null,\"OptionQuestions\":null,\"AppDates\":null,\"PersonAgeTypes\":null,\"SendMailState\":{\"BookingReminderOfUpcomingCheckInDateSend\":null,\"BookingReminderOfUpcomingCheckInDateSent\":null,\"BookingCancellationFeeReminderSend\":null,\"BookingCancellationFeeReminderSent\":null},\"TotalRoomPrice\":0.0,\"TotalSpaTax\":0.0,\"TotalOptionPrice\":0.0,\"UsedPoint\":0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"TotalPrice\":0.0,\"IsSiteLocation\":false,\"TimeZoneOffset\":0,\"MediaCode\":null,\"RootCode\":null}");

            migrationBuilder.AddColumn<string>(
                name: "notice_number",
                schema: "public",
                table: "reservation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "notice_number",
                schema: "public",
                table: "reservation");

            migrationBuilder.AlterColumn<string>(
                name: "booking_data",
                schema: "public",
                table: "reservation",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"LanguageCode\":null,\"Facility\":{\"Id\":null,\"Name\":null,\"Address\":\"\",\"FullAddress\":\"\",\"Address1\":null,\"Address2\":null,\"Address3\":null,\"Address4\":null,\"Tel\":null,\"CanOnLinePayment\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null},\"Plan\":{\"Id\":null,\"Name\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null,\"DayUse\":false,\"Meta\":null,\"Meals\":null,\"Files\":null,\"IsCancelSameAccept\":false,\"CancelDayLimit\":null,\"ReceptionDayLimit\":null,\"CancelLimit\":null,\"CancellationDataPolicy\":null},\"RoomGroup\":{\"Id\":null,\"Name\":null,\"GroupName\":null,\"IsEnabledSmoking\":null,\"CapacityMax\":null,\"CapacityMin\":null,\"Files\":null},\"Site\":{\"Id\":null,\"Name\":null,\"ShortName\":null},\"PlanQuestions\":null,\"OptionQuestions\":null,\"AppDates\":null,\"PersonAgeTypes\":null,\"SendMailState\":{\"BookingReminderOfUpcomingCheckInDateSend\":null,\"BookingReminderOfUpcomingCheckInDateSent\":null,\"BookingCancellationFeeReminderSend\":null,\"BookingCancellationFeeReminderSent\":null},\"TotalRoomPrice\":0.0,\"TotalSpaTax\":0.0,\"TotalOptionPrice\":0.0,\"UsedPoint\":0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"TotalPrice\":0.0,\"IsSiteLocation\":false,\"TimeZoneOffset\":0,\"MediaCode\":null,\"RootCode\":null}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"LanguageCode\":null,\"Facility\":{\"Id\":null,\"Name\":null,\"Address\":\"\",\"FullAddress\":\"\",\"Address1\":null,\"Address2\":null,\"Address3\":null,\"Address4\":null,\"Tel\":null,\"CanOnLinePayment\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null},\"Plan\":{\"Id\":null,\"Name\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null,\"DayUse\":false,\"Meta\":null,\"Meals\":null,\"Files\":null,\"IsCancelSameAccept\":false,\"CancelDayLimit\":null,\"ReceptionDayLimit\":null,\"CancelLimit\":null,\"CancellationDataPolicy\":null},\"RoomGroup\":{\"Id\":null,\"Name\":null,\"GroupName\":null,\"IsEnabledSmoking\":null,\"CapacityMax\":null,\"CapacityMin\":null,\"Files\":null},\"Site\":{\"Id\":null,\"Name\":null,\"ShortName\":null,\"PrefixName\":null},\"PlanQuestions\":null,\"OptionQuestions\":null,\"AppDates\":null,\"PersonAgeTypes\":null,\"SendMailState\":{\"BookingReminderOfUpcomingCheckInDateSend\":null,\"BookingReminderOfUpcomingCheckInDateSent\":null,\"BookingCancellationFeeReminderSend\":null,\"BookingCancellationFeeReminderSent\":null},\"TotalRoomPrice\":0.0,\"TotalSpaTax\":0.0,\"TotalOptionPrice\":0.0,\"UsedPoint\":0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"TotalPrice\":0.0,\"IsSiteLocation\":false,\"TimeZoneOffset\":0,\"MediaCode\":null,\"RootCode\":null}");
        }
    }
}
