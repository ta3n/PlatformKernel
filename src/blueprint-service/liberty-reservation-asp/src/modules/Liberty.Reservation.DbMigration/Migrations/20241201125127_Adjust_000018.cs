using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000018 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "template_format_json",
                schema: "public",
                table: "system_config");

            migrationBuilder.DropColumn(
                name: "meta_json",
                schema: "public",
                table: "person_age_type");

            migrationBuilder.AddColumn<string>(
                name: "template_format_data",
                schema: "public",
                table: "system_config",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"Io10001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10002TemplateFormat\":{\"Subject\":\"*IO10002 ご予約成立\",\"Body\":\"*IO10002 本メールは宿泊施設向けの内容です。\\n本予約は予約成立されました\",\"Url\":\"http://\"},\"Io10003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10005TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10006TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10007TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10008TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10009TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10010TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10101TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10102TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10201TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10202TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10203TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10204TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\",\"Url\":\"\"},\"Io10205TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10206TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10207TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io20001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"}}");

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
                oldDefaultValue: "{\"Facility\":{\"Id\":null,\"Name\":null,\"Address\":null,\"Address1\":null,\"Address2\":null,\"Address3\":null,\"Address4\":null,\"CanOnLinePayment\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null},\"Plan\":{\"Id\":null,\"Name\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null,\"Meta\":null,\"Files\":null},\"RoomGroup\":{\"Id\":null,\"Name\":null,\"IsEnabledSmoking\":null,\"CapacityMax\":null,\"CapacityMin\":null,\"Files\":null},\"Site\":{\"Id\":null,\"Name\":null},\"PlanQuestions\":null,\"OptionQuestions\":null,\"AppDates\":null,\"SendMailState\":{\"BookingReminderOfUpcomingCheckInDateSend\":null,\"BookingReminderOfUpcomingCheckInDateSent\":null,\"BookingCancellationFeeReminderSend\":null,\"BookingCancellationFeeReminderSent\":null},\"TotalRoomPrice\":0.0,\"TotalSpaTax\":0.0,\"TotalOptionPrice\":0.0,\"UsedPoint\":0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"TotalPrice\":0.0}");

            migrationBuilder.AddColumn<string>(
                name: "meta",
                schema: "public",
                table: "person_age_type",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"PersonAgeGroup\":0,\"FoodBed\":0}");

            migrationBuilder.AddColumn<string>(
                name: "address1",
                schema: "public",
                table: "facility",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address2",
                schema: "public",
                table: "facility",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address3",
                schema: "public",
                table: "facility",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address4",
                schema: "public",
                table: "facility",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kana",
                schema: "public",
                table: "facility",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                schema: "public",
                table: "facility",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "postcode",
                schema: "public",
                table: "facility",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "template_format_data",
                schema: "public",
                table: "system_config");

            migrationBuilder.DropColumn(
                name: "meta",
                schema: "public",
                table: "person_age_type");

            migrationBuilder.DropColumn(
                name: "address1",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "address2",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "address3",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "address4",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "kana",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "name",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "postcode",
                schema: "public",
                table: "facility");

            migrationBuilder.AddColumn<string>(
                name: "template_format_json",
                schema: "public",
                table: "system_config",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "booking_data",
                schema: "public",
                table: "reservation",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"Facility\":{\"Id\":null,\"Name\":null,\"Address\":null,\"Address1\":null,\"Address2\":null,\"Address3\":null,\"Address4\":null,\"CanOnLinePayment\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null},\"Plan\":{\"Id\":null,\"Name\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null,\"Meta\":null,\"Files\":null},\"RoomGroup\":{\"Id\":null,\"Name\":null,\"IsEnabledSmoking\":null,\"CapacityMax\":null,\"CapacityMin\":null,\"Files\":null},\"Site\":{\"Id\":null,\"Name\":null},\"PlanQuestions\":null,\"OptionQuestions\":null,\"AppDates\":null,\"SendMailState\":{\"BookingReminderOfUpcomingCheckInDateSend\":null,\"BookingReminderOfUpcomingCheckInDateSent\":null,\"BookingCancellationFeeReminderSend\":null,\"BookingCancellationFeeReminderSent\":null},\"TotalRoomPrice\":0.0,\"TotalSpaTax\":0.0,\"TotalOptionPrice\":0.0,\"UsedPoint\":0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"TotalPrice\":0.0}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"Facility\":{\"Id\":null,\"Name\":null,\"Address\":null,\"Address1\":null,\"Address2\":null,\"Address3\":null,\"Address4\":null,\"CanOnLinePayment\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null},\"Plan\":{\"Id\":null,\"Name\":null,\"IsOnSidePayment\":null,\"IsOnLinePayment\":null,\"Meta\":null,\"Files\":null},\"RoomGroup\":{\"Id\":null,\"Name\":null,\"IsEnabledSmoking\":null,\"CapacityMax\":null,\"CapacityMin\":null,\"Files\":null},\"Site\":{\"Id\":null,\"Name\":null},\"PlanQuestions\":null,\"OptionQuestions\":null,\"AppDates\":null,\"PersonAgeTypes\":null,\"SendMailState\":{\"BookingReminderOfUpcomingCheckInDateSend\":null,\"BookingReminderOfUpcomingCheckInDateSent\":null,\"BookingCancellationFeeReminderSend\":null,\"BookingCancellationFeeReminderSent\":null},\"TotalRoomPrice\":0.0,\"TotalSpaTax\":0.0,\"TotalOptionPrice\":0.0,\"UsedPoint\":0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"TotalPrice\":0.0}");

            migrationBuilder.AddColumn<string>(
                name: "meta_json",
                schema: "public",
                table: "person_age_type",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
