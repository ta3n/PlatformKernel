using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_00003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_info_reservation_room_group_app_date_person_age_type_r",
                schema: "public",
                table: "user_info");

            migrationBuilder.DropIndex(
                name: "ix_user_info_reservation_room_group_app_date_person_age_type_r",
                schema: "public",
                table: "user_info");

            migrationBuilder.DropColumn(
                name: "reservation_room_group_app_date_person_age_type_app_date_id",
                schema: "public",
                table: "user_info");

            migrationBuilder.DropColumn(
                name: "reservation_room_group_app_date_person_age_type_person_age_type_id",
                schema: "public",
                table: "user_info");

            migrationBuilder.DropColumn(
                name: "reservation_room_group_app_date_person_age_type_reservation_id",
                schema: "public",
                table: "user_info");

            migrationBuilder.DropColumn(
                name: "reservation_room_group_app_date_person_age_type_room_group_id",
                schema: "public",
                table: "user_info");

            migrationBuilder.DropColumn(
                name: "reservation_room_group_app_date_person_age_type_room_group_index",
                schema: "public",
                table: "user_info");

            migrationBuilder.AddColumn<int>(
                name: "female_number",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "gender_none_number",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "male_number",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "reservation_plan_room_group_app_date_app_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "reservation_plan_room_group_app_date_plan_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "reservation_plan_room_group_app_date_reservation_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "reservation_plan_room_group_app_date_room_group_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "reservation_plan_room_group_app_date_room_group_index",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_group_app_date_person_age_type_reservation",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                columns: new[] { "reservation_plan_room_group_app_date_reservation_id", "reservation_plan_room_group_app_date_plan_id", "reservation_plan_room_group_app_date_room_group_id", "reservation_plan_room_group_app_date_app_date_id", "reservation_plan_room_group_app_date_room_group_index" });

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_reservation1",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                columns: new[] { "reservation_plan_room_group_app_date_reservation_id", "reservation_plan_room_group_app_date_plan_id", "reservation_plan_room_group_app_date_room_group_id", "reservation_plan_room_group_app_date_app_date_id", "reservation_plan_room_group_app_date_room_group_index" },
                principalSchema: "public",
                principalTable: "reservation_plan_room_group_app_date",
                principalColumns: new[] { "reservation_id", "plan_id", "room_group_id", "app_date_id", "room_group_index" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_reservation1",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropIndex(
                name: "ix_reservation_room_group_app_date_person_age_type_reservation",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropColumn(
                name: "female_number",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropColumn(
                name: "gender_none_number",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropColumn(
                name: "male_number",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropColumn(
                name: "reservation_plan_room_group_app_date_app_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropColumn(
                name: "reservation_plan_room_group_app_date_plan_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropColumn(
                name: "reservation_plan_room_group_app_date_reservation_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropColumn(
                name: "reservation_plan_room_group_app_date_room_group_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropColumn(
                name: "reservation_plan_room_group_app_date_room_group_index",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.AddColumn<long>(
                name: "reservation_room_group_app_date_person_age_type_app_date_id",
                schema: "public",
                table: "user_info",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "reservation_room_group_app_date_person_age_type_person_age_type_id",
                schema: "public",
                table: "user_info",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "reservation_room_group_app_date_person_age_type_reservation_id",
                schema: "public",
                table: "user_info",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "reservation_room_group_app_date_person_age_type_room_group_id",
                schema: "public",
                table: "user_info",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "reservation_room_group_app_date_person_age_type_room_group_index",
                schema: "public",
                table: "user_info",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_info_reservation_room_group_app_date_person_age_type_r",
                schema: "public",
                table: "user_info",
                columns: new[] { "reservation_room_group_app_date_person_age_type_reservation_id", "reservation_room_group_app_date_person_age_type_room_group_id", "reservation_room_group_app_date_person_age_type_app_date_id", "reservation_room_group_app_date_person_age_type_person_age_type_id", "reservation_room_group_app_date_person_age_type_room_group_index" });

            migrationBuilder.AddForeignKey(
                name: "fk_user_info_reservation_room_group_app_date_person_age_type_r",
                schema: "public",
                table: "user_info",
                columns: new[] { "reservation_room_group_app_date_person_age_type_reservation_id", "reservation_room_group_app_date_person_age_type_room_group_id", "reservation_room_group_app_date_person_age_type_app_date_id", "reservation_room_group_app_date_person_age_type_person_age_type_id", "reservation_room_group_app_date_person_age_type_room_group_index" },
                principalSchema: "public",
                principalTable: "reservation_room_group_app_date_person_age_type",
                principalColumns: new[] { "reservation_id", "room_group_id", "app_date_id", "person_age_type_id", "room_group_index" });
        }
    }
}
