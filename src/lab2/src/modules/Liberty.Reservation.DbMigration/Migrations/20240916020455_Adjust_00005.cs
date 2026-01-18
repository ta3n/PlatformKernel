using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_00005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "json_data",
                schema: "public",
                table: "reservation");

            migrationBuilder.AddColumn<string>(
                name: "reservation_data",
                schema: "public",
                table: "reservation",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserId\":0,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"UserDatas\":null,\"PersonAgeTypes\":null,\"ReservationPersonDatas\":null,\"ReservationPriceDatas\":null,\"ReservationOptionItems\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0,\"TotalDiscount\":0,\"AllTotalPrice\":0,\"HasOptionItem\":false}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reservation_data",
                schema: "public",
                table: "reservation");

            migrationBuilder.AddColumn<string>(
                name: "json_data",
                schema: "public",
                table: "reservation",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
