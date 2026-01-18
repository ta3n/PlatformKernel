using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000044 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_enabled_minimum_price",
                schema: "public",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "minimum_price",
                schema: "public",
                table: "plan");

            migrationBuilder.AlterColumn<string>(
                name: "template_format_data",
                schema: "public",
                table: "system_config",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"Io10001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10001EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10002TemplateFormat\":{\"Subject\":\"*IO10002 ご予約成立\",\"Body\":\"*IO10002 本メールは宿泊施設向けの内容です。\\n本予約は予約成立されました\",\"Url\":\"http://\"},\"Io10003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10004EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10005TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10006TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10006EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10007TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10008TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10008EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10009TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10010TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10010EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10011TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10011EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10012TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10012EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10013TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10014TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10014EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10015TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10015EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10101TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10101EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10102TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10102EnTemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10103TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10103EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10104TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10104EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10201TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10202TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10203TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10204TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\",\"Url\":\"\"},\"Io10205TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10206TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10207TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io20001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"}}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"Io10001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10001EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10002TemplateFormat\":{\"Subject\":\"*IO10002 ご予約成立\",\"Body\":\"*IO10002 本メールは宿泊施設向けの内容です。\\n本予約は予約成立されました\",\"Url\":\"http://\"},\"Io10003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10004EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10005TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10006TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10006EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10007TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10008TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10008EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10009TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10010TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10010EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10011TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10011EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10012TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10012EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10013TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10101TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10101EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10102TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10102EnTemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10103TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10103EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10104TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10104EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10201TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10202TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10203TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10204TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\",\"Url\":\"\"},\"Io10205TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10206TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10207TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io20001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"}}");

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled_minimum_price",
                schema: "public",
                table: "plan_room_group_site",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "minimum_price",
                schema: "public",
                table: "plan_room_group_site",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_enabled_minimum_price",
                schema: "public",
                table: "plan_room_group_site");

            migrationBuilder.DropColumn(
                name: "minimum_price",
                schema: "public",
                table: "plan_room_group_site");

            migrationBuilder.AlterColumn<string>(
                name: "template_format_data",
                schema: "public",
                table: "system_config",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"Io10001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10001EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10002TemplateFormat\":{\"Subject\":\"*IO10002 ご予約成立\",\"Body\":\"*IO10002 本メールは宿泊施設向けの内容です。\\n本予約は予約成立されました\",\"Url\":\"http://\"},\"Io10003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10004EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10005TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10006TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10006EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10007TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10008TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10008EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10009TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10010TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10010EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10011TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10011EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10012TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10012EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10013TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10101TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10101EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10102TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10102EnTemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10103TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10103EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10104TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10104EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10201TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10202TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10203TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10204TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\",\"Url\":\"\"},\"Io10205TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10206TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10207TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io20001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"}}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"Io10001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10001EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10002TemplateFormat\":{\"Subject\":\"*IO10002 ご予約成立\",\"Body\":\"*IO10002 本メールは宿泊施設向けの内容です。\\n本予約は予約成立されました\",\"Url\":\"http://\"},\"Io10003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10004EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10005TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10006TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10006EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10007TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10008TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10008EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10009TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10010TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10010EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10011TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10011EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10012TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10012EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10013TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10014TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10014EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10015TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10015EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10101TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10101EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10102TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10102EnTemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10103TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10103EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10104TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10104EnTemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10201TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io10202TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10203TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10204TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\",\"Url\":\"\"},\"Io10205TemplateFormat\":{\"Subject\":\"*\",\"Body\":\"*\"},\"Io10206TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io10207TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\",\"Url\":\"\"},\"Io20001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io20004TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30001TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30002TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"},\"Io30003TemplateFormat\":{\"Subject\":\"\",\"Body\":\"\"}}");

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled_minimum_price",
                schema: "public",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "minimum_price",
                schema: "public",
                table: "plan",
                type: "integer",
                nullable: true);
        }
    }
}
