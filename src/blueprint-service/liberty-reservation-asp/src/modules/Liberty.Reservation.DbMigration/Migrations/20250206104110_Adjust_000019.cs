using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000019 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "template_format_data",
                schema: "public",
                table: "system_config",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"Io10001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10002TemplateFormat\":{\"Subject\":\"*IO10002 ご予約成立\",\"Body\":\"*IO10002 本メールは宿泊施設向けの内容です。\\n本予約は予約成立されました\",\"Url\":\"http://\"},\"Io10003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10005TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10006TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10007TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10008TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10009TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10010TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10011TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10012TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10101TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10102TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10103TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10104TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10201TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10202TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10203TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10204TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\",\"Url\":\"\"},\"Io10205TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10206TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10207TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io20001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"}}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"Io10001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10002TemplateFormat\":{\"Subject\":\"*IO10002 ご予約成立\",\"Body\":\"*IO10002 本メールは宿泊施設向けの内容です。\\n本予約は予約成立されました\",\"Url\":\"http://\"},\"Io10003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10005TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10006TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10007TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10008TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10009TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10010TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10101TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10102TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10201TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10202TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10203TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10204TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\",\"Url\":\"\"},\"Io10205TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10206TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10207TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io20001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"}}");

            migrationBuilder.AlterColumn<string>(
                name: "booking_data",
                schema: "public",
                table: "reservation",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"Facility\":{\"Id\":null,\"Name\":null,\"Address\":\"\",\"Address1\":null,\"Address2\":null,\"Address3\":null,\"Address4\":null,\"CanOnLinePayment\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null},\"Plan\":{\"Id\":null,\"Name\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null,\"Meta\":null,\"Files\":null},\"RoomGroup\":{\"Id\":null,\"Name\":null,\"IsEnabledSmoking\":null,\"CapacityMax\":null,\"CapacityMin\":null,\"Files\":null},\"Site\":{\"Id\":null,\"Name\":null},\"PlanQuestions\":null,\"OptionQuestions\":null,\"AppDates\":null,\"PersonAgeTypes\":null,\"SendMailState\":{\"BookingReminderOfUpcomingCheckInDateSend\":null,\"BookingReminderOfUpcomingCheckInDateSent\":null,\"BookingCancellationFeeReminderSend\":null,\"BookingCancellationFeeReminderSent\":null},\"TotalRoomPrice\":0.0,\"TotalSpaTax\":0.0,\"TotalOptionPrice\":0.0,\"UsedPoint\":0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"TotalPrice\":0.0,\"IsSiteLocation\":false}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"Facility\":{\"Id\":null,\"Name\":null,\"Address\":null,\"Address1\":null,\"Address2\":null,\"Address3\":null,\"Address4\":null,\"CanOnLinePayment\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null},\"Plan\":{\"Id\":null,\"Name\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null,\"Meta\":null,\"Files\":null},\"RoomGroup\":{\"Id\":null,\"Name\":null,\"IsEnabledSmoking\":null,\"CapacityMax\":null,\"CapacityMin\":null,\"Files\":null},\"Site\":{\"Id\":null,\"Name\":null},\"PlanQuestions\":null,\"OptionQuestions\":null,\"AppDates\":null,\"PersonAgeTypes\":null,\"SendMailState\":{\"BookingReminderOfUpcomingCheckInDateSend\":null,\"BookingReminderOfUpcomingCheckInDateSent\":null,\"BookingCancellationFeeReminderSend\":null,\"BookingCancellationFeeReminderSent\":null},\"TotalRoomPrice\":0.0,\"TotalSpaTax\":0.0,\"TotalOptionPrice\":0.0,\"UsedPoint\":0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"TotalPrice\":0.0}");

            migrationBuilder.AlterColumn<string>(
                name: "meta",
                schema: "public",
                table: "person_age_type",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"PersonAgeGroup\":0,\"FoodBed\":0,\"GroupName\":null}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"PersonAgeGroup\":0,\"FoodBed\":0}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "template_format_data",
                schema: "public",
                table: "system_config",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"Io10001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10002TemplateFormat\":{\"Subject\":\"*IO10002 ご予約成立\",\"Body\":\"*IO10002 本メールは宿泊施設向けの内容です。\\n本予約は予約成立されました\",\"Url\":\"http://\"},\"Io10003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10005TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10006TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10007TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10008TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10009TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10010TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10101TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10102TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10201TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10202TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10203TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10204TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\",\"Url\":\"\"},\"Io10205TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10206TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10207TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io20001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"}}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"Io10001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10002TemplateFormat\":{\"Subject\":\"*IO10002 ご予約成立\",\"Body\":\"*IO10002 本メールは宿泊施設向けの内容です。\\n本予約は予約成立されました\",\"Url\":\"http://\"},\"Io10003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10005TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10006TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10007TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10008TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10009TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10010TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10011TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10012TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10101TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10102TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10103TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10104TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10201TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10202TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10203TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10204TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\",\"Url\":\"\"},\"Io10205TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10206TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10207TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io20001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"}}");

            migrationBuilder.AlterColumn<string>(
                name: "booking_data",
                schema: "public",
                table: "reservation",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"Facility\":{\"Id\":null,\"Name\":null,\"Address\":null,\"Address1\":null,\"Address2\":null,\"Address3\":null,\"Address4\":null,\"CanOnLinePayment\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null},\"Plan\":{\"Id\":null,\"Name\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null,\"Meta\":null,\"Files\":null},\"RoomGroup\":{\"Id\":null,\"Name\":null,\"IsEnabledSmoking\":null,\"CapacityMax\":null,\"CapacityMin\":null,\"Files\":null},\"Site\":{\"Id\":null,\"Name\":null},\"PlanQuestions\":null,\"OptionQuestions\":null,\"AppDates\":null,\"PersonAgeTypes\":null,\"SendMailState\":{\"BookingReminderOfUpcomingCheckInDateSend\":null,\"BookingReminderOfUpcomingCheckInDateSent\":null,\"BookingCancellationFeeReminderSend\":null,\"BookingCancellationFeeReminderSent\":null},\"TotalRoomPrice\":0.0,\"TotalSpaTax\":0.0,\"TotalOptionPrice\":0.0,\"UsedPoint\":0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"TotalPrice\":0.0}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"Facility\":{\"Id\":null,\"Name\":null,\"Address\":\"\",\"Address1\":null,\"Address2\":null,\"Address3\":null,\"Address4\":null,\"CanOnLinePayment\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null},\"Plan\":{\"Id\":null,\"Name\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null,\"Meta\":null,\"Files\":null},\"RoomGroup\":{\"Id\":null,\"Name\":null,\"IsEnabledSmoking\":null,\"CapacityMax\":null,\"CapacityMin\":null,\"Files\":null},\"Site\":{\"Id\":null,\"Name\":null},\"PlanQuestions\":null,\"OptionQuestions\":null,\"AppDates\":null,\"PersonAgeTypes\":null,\"SendMailState\":{\"BookingReminderOfUpcomingCheckInDateSend\":null,\"BookingReminderOfUpcomingCheckInDateSent\":null,\"BookingCancellationFeeReminderSend\":null,\"BookingCancellationFeeReminderSent\":null},\"TotalRoomPrice\":0.0,\"TotalSpaTax\":0.0,\"TotalOptionPrice\":0.0,\"UsedPoint\":0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"TotalPrice\":0.0,\"IsSiteLocation\":false}");

            migrationBuilder.AlterColumn<string>(
                name: "meta",
                schema: "public",
                table: "person_age_type",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"PersonAgeGroup\":0,\"FoodBed\":0}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"PersonAgeGroup\":0,\"FoodBed\":0,\"GroupName\":null}");
        }
    }
}
