using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_00009 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservation_customer_info_main_user_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_plan_room_group_app_date_app_date_app_date_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_plan_room_group_app_date_customer_info_user_inf",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_plan_room_group_app_date_room_group_app_date_ro",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_option_item_app_date_app_da",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_option_item_reservation_pla",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_app_date_ap",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_reservation1",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropForeignKey(
                name: "fk_user_customer_info_user_info_id",
                schema: "public",
                table: "user");

            migrationBuilder.DropIndex(
                name: "ix_user_user_info_id",
                schema: "public",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_reservation_room_group_app_date_person_age_type",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropPrimaryKey(
                name: "pk_reservation_room_group_app_date_option_item",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_reservation_plan_room_group_app_date",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropIndex(
                name: "ix_reservation_plan_room_group_app_date_room_group_id_app_date",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropIndex(
                name: "ix_reservation_plan_room_group_app_date_user_info_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropColumn(
                name: "meta_json",
                schema: "public",
                table: "option_item");

            migrationBuilder.DropColumn(
                name: "meta_json",
                schema: "public",
                table: "file");

            migrationBuilder.RenameColumn(
                name: "reservation_plan_room_group_app_date_app_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                newName: "reservation_plan_room_group_app_date_booking_date_id");

            migrationBuilder.RenameColumn(
                name: "reservation_plan_room_group_app_date_app_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                newName: "reservation_plan_room_group_app_date_booking_date_id");

            migrationBuilder.RenameColumn(
                name: "user_info_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                newName: "room_group_app_date_room_group_id");

            migrationBuilder.AddColumn<long>(
                name: "customer_info_id",
                schema: "public",
                table: "user",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "app_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "booking_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "app_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "booking_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "app_date_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "booking_date_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "customer_info_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "room_group_app_date_app_date_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "user_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

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
                oldDefaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserId\":0,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0.0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"HasOptionItem\":false}");

            migrationBuilder.AlterColumn<long>(
                name: "main_user_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "user_code",
                schema: "public",
                table: "reservation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "country_code",
                schema: "public",
                table: "customer_info",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_reservation_room_group_app_date_person_age_type",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                columns: new[] { "reservation_id", "room_group_id", "booking_date_id", "person_age_type_id", "room_group_index" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_reservation_room_group_app_date_option_item",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                columns: new[] { "reservation_id", "room_group_id", "booking_date_id", "option_item_id", "room_group_index" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_reservation_plan_room_group_app_date",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                columns: new[] { "reservation_id", "plan_id", "room_group_id", "booking_date_id", "room_group_index" });

            migrationBuilder.CreateIndex(
                name: "ix_user_customer_info_id",
                schema: "public",
                table: "user",
                column: "customer_info_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_plan_room_group_app_date_customer_info_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                column: "customer_info_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_plan_room_group_app_date_room_group_app_date_ro",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                columns: new[] { "room_group_app_date_room_group_id", "room_group_app_date_app_date_id" });

            migrationBuilder.CreateIndex(
                name: "ix_reservation_plan_room_group_app_date_room_group_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                column: "room_group_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_customer_info_main_user_id",
                schema: "public",
                table: "reservation",
                column: "main_user_id",
                principalSchema: "public",
                principalTable: "customer_info",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_plan_room_group_app_date_app_date_app_date_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                column: "app_date_id",
                principalSchema: "public",
                principalTable: "app_date",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_plan_room_group_app_date_customer_info_customer",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                column: "customer_info_id",
                principalSchema: "public",
                principalTable: "customer_info",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_plan_room_group_app_date_room_group_app_date_ro",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                columns: new[] { "room_group_app_date_room_group_id", "room_group_app_date_app_date_id" },
                principalSchema: "public",
                principalTable: "room_group_app_date",
                principalColumns: new[] { "room_group_id", "app_date_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_room_group_app_date_option_item_app_date_app_da",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                column: "app_date_id",
                principalSchema: "public",
                principalTable: "app_date",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_room_group_app_date_option_item_reservation_pla",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                columns: new[] { "reservation_plan_room_group_app_date_reservation_id", "reservation_plan_room_group_app_date_plan_id", "reservation_plan_room_group_app_date_room_group_id", "reservation_plan_room_group_app_date_booking_date_id", "reservation_plan_room_group_app_date_room_group_index" },
                principalSchema: "public",
                principalTable: "reservation_plan_room_group_app_date",
                principalColumns: new[] { "reservation_id", "plan_id", "room_group_id", "booking_date_id", "room_group_index" });

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_app_date_ap",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                column: "app_date_id",
                principalSchema: "public",
                principalTable: "app_date",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_reservation1",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                columns: new[] { "reservation_plan_room_group_app_date_reservation_id", "reservation_plan_room_group_app_date_plan_id", "reservation_plan_room_group_app_date_room_group_id", "reservation_plan_room_group_app_date_booking_date_id", "reservation_plan_room_group_app_date_room_group_index" },
                principalSchema: "public",
                principalTable: "reservation_plan_room_group_app_date",
                principalColumns: new[] { "reservation_id", "plan_id", "room_group_id", "booking_date_id", "room_group_index" });

            migrationBuilder.AddForeignKey(
                name: "fk_user_customer_info_customer_info_id",
                schema: "public",
                table: "user",
                column: "customer_info_id",
                principalSchema: "public",
                principalTable: "customer_info",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservation_customer_info_main_user_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_plan_room_group_app_date_app_date_app_date_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_plan_room_group_app_date_customer_info_customer",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_plan_room_group_app_date_room_group_app_date_ro",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_option_item_app_date_app_da",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_option_item_reservation_pla",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_app_date_ap",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_reservation1",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropForeignKey(
                name: "fk_user_customer_info_customer_info_id",
                schema: "public",
                table: "user");

            migrationBuilder.DropIndex(
                name: "ix_user_customer_info_id",
                schema: "public",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_reservation_room_group_app_date_person_age_type",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropPrimaryKey(
                name: "pk_reservation_room_group_app_date_option_item",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_reservation_plan_room_group_app_date",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropIndex(
                name: "ix_reservation_plan_room_group_app_date_customer_info_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropIndex(
                name: "ix_reservation_plan_room_group_app_date_room_group_app_date_ro",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropIndex(
                name: "ix_reservation_plan_room_group_app_date_room_group_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropColumn(
                name: "customer_info_id",
                schema: "public",
                table: "user");

            migrationBuilder.DropColumn(
                name: "booking_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropColumn(
                name: "booking_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item");

            migrationBuilder.DropColumn(
                name: "booking_date_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropColumn(
                name: "customer_info_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropColumn(
                name: "room_group_app_date_app_date_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date");

            migrationBuilder.DropColumn(
                name: "user_code",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropColumn(
                name: "country_code",
                schema: "public",
                table: "customer_info");

            migrationBuilder.RenameColumn(
                name: "reservation_plan_room_group_app_date_booking_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                newName: "reservation_plan_room_group_app_date_app_date_id");

            migrationBuilder.RenameColumn(
                name: "reservation_plan_room_group_app_date_booking_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                newName: "reservation_plan_room_group_app_date_app_date_id");

            migrationBuilder.RenameColumn(
                name: "room_group_app_date_room_group_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                newName: "user_info_id");

            migrationBuilder.AlterColumn<long>(
                name: "app_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "app_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "app_date_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "user_id",
                schema: "public",
                table: "reservation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

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
                oldDefaultValue: "{\"Code\":null,\"CheckInDate\":0,\"RestNumber\":0,\"RoomNumber\":0,\"CheckInTime\":null,\"CheckOutTime\":null,\"IsSameMainUser\":false,\"UseRoomUser\":false,\"Facility\":null,\"Plan\":null,\"RoomGroup\":null,\"Site\":null,\"UserCode\":null,\"Reserver\":null,\"MainUser\":null,\"PaymentType\":null,\"UsedPoint\":0,\"IncomePoint\":0,\"Memo\":null,\"ReservationPlanRoomGroupAppDates\":null,\"ReservationPlanQuestions\":null,\"ReservationOptionItemQuestions\":null,\"Parent\":null,\"Children\":null,\"TotalRoomGroupPrice\":null,\"TotalSpaTax\":null,\"TotalOptionItemPrice\":null,\"TotalPrice\":0.0,\"TotalDiscount\":0.0,\"AllTotalPrice\":0.0,\"HasOptionItem\":false}");

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

            migrationBuilder.AddColumn<string>(
                name: "meta_json",
                schema: "public",
                table: "option_item",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "meta_json",
                schema: "public",
                table: "file",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_reservation_room_group_app_date_person_age_type",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                columns: new[] { "reservation_id", "room_group_id", "app_date_id", "person_age_type_id", "room_group_index" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_reservation_room_group_app_date_option_item",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                columns: new[] { "reservation_id", "room_group_id", "app_date_id", "option_item_id", "room_group_index" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_reservation_plan_room_group_app_date",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                columns: new[] { "reservation_id", "plan_id", "room_group_id", "app_date_id", "room_group_index" });

            migrationBuilder.CreateIndex(
                name: "ix_user_user_info_id",
                schema: "public",
                table: "user",
                column: "user_info_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_plan_room_group_app_date_room_group_id_app_date",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                columns: new[] { "room_group_id", "app_date_id" });

            migrationBuilder.CreateIndex(
                name: "ix_reservation_plan_room_group_app_date_user_info_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                column: "user_info_id");

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
                name: "fk_reservation_plan_room_group_app_date_app_date_app_date_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                column: "app_date_id",
                principalSchema: "public",
                principalTable: "app_date",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_plan_room_group_app_date_customer_info_user_inf",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                column: "user_info_id",
                principalSchema: "public",
                principalTable: "customer_info",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_plan_room_group_app_date_room_group_app_date_ro",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                columns: new[] { "room_group_id", "app_date_id" },
                principalSchema: "public",
                principalTable: "room_group_app_date",
                principalColumns: new[] { "room_group_id", "app_date_id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_room_group_app_date_option_item_app_date_app_da",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                column: "app_date_id",
                principalSchema: "public",
                principalTable: "app_date",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_room_group_app_date_option_item_reservation_pla",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                columns: new[] { "reservation_plan_room_group_app_date_reservation_id", "reservation_plan_room_group_app_date_plan_id", "reservation_plan_room_group_app_date_room_group_id", "reservation_plan_room_group_app_date_app_date_id", "reservation_plan_room_group_app_date_room_group_index" },
                principalSchema: "public",
                principalTable: "reservation_plan_room_group_app_date",
                principalColumns: new[] { "reservation_id", "plan_id", "room_group_id", "app_date_id", "room_group_index" });

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_app_date_ap",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                column: "app_date_id",
                principalSchema: "public",
                principalTable: "app_date",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_reservation1",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                columns: new[] { "reservation_plan_room_group_app_date_reservation_id", "reservation_plan_room_group_app_date_plan_id", "reservation_plan_room_group_app_date_room_group_id", "reservation_plan_room_group_app_date_app_date_id", "reservation_plan_room_group_app_date_room_group_index" },
                principalSchema: "public",
                principalTable: "reservation_plan_room_group_app_date",
                principalColumns: new[] { "reservation_id", "plan_id", "room_group_id", "app_date_id", "room_group_index" });

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
    }
}
