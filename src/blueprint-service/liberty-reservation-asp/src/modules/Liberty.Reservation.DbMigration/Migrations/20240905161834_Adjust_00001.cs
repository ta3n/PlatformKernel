using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_00001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<SiteMeta>(
                name: "meta",
                schema: "public",
                table: "site",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "json",
                oldNullable: true,
                oldDefaultValue: "{\"RSSItemLinkFormat\":null}");

            migrationBuilder.AlterColumn<PlanMeta>(
                name: "meta",
                schema: "public",
                table: "plan",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "json",
                oldNullable: true,
                oldDefaultValue: "{\"Heading1\":null,\"Summary\":null,\"Description\":null,\"Payment\":null,\"Meal\":null,\"BarrierFree\":null,\"SpaTax\":null,\"SpaTaxTable\":null,\"Cancelling\":null,\"CancellingTable\":null,\"Other\":null}");

            migrationBuilder.AlterColumn<FacilityMeta>(
                name: "meta",
                schema: "public",
                table: "facility",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "json",
                oldNullable: true,
                oldDefaultValue: "{\"RoomNumberWesternStyle\":null,\"RoomNumberJapaneseStyle\":null,\"RoomNumberJapaneseWesternStyle\":null,\"RoomNumberOtherStyle\":null,\"ReceptionLimit\":\"00:00:00\",\"CheckInStart\":\"00:00:00\",\"CheckInEnd\":\"00:00:00\",\"CheckOut\":\"00:00:00\",\"Heading1\":null,\"PRPointComment\":null,\"CancellingComment\":null,\"CancellingTable\":null,\"MapUrl\":null,\"UseSpaTax\":false,\"SpaTaxComment\":null,\"SpaTaxTable\":null,\"SpaType\":null,\"SpaName\":null,\"SpaDescription\":null,\"SpaInfoComment\":null,\"IsAcceptChildren\":false,\"AcceptChildrenInfoComment\":null,\"IsAcceptPet\":false,\"AcceptPetInfoComment\":null,\"IsBarrierFree\":false,\"BarrierFreeInfoComment\":null,\"MealTypeComment\":null,\"OnSidePaymentComment\":null,\"OnLinePaymentComment\":null,\"PaymentComment\":null,\"AccessInfoComment\":null,\"NearStationInfoComment\":null,\"ExistsParking\":false,\"ParkingInfoComment\":null,\"CanTransfer\":false,\"TransferComment\":null,\"EquipmentInfoComment\":null,\"RoomInfoComment\":null,\"AmenityInfoComment\":null,\"LeisureInfoComment\":null,\"FAQInfoComment\":null,\"OtherInfoComment\":null,\"UseWarnPrice\":false,\"WarnPrice\":0,\"UseWarnPricePercent\":false,\"WarnPricePercent\":0,\"OperationMode\":0,\"FileStorageLimit\":0.0,\"UseImportPlanName\":false,\"SEOCoefficient\":0.0,\"SystemEMail\":null}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "meta",
                schema: "public",
                table: "site",
                type: "json",
                nullable: true,
                defaultValue: "{\"RSSItemLinkFormat\":null}",
                oldClrType: typeof(SiteMeta),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "meta",
                schema: "public",
                table: "plan",
                type: "json",
                nullable: true,
                defaultValue: "{\"Heading1\":null,\"Summary\":null,\"Description\":null,\"Payment\":null,\"Meal\":null,\"BarrierFree\":null,\"SpaTax\":null,\"SpaTaxTable\":null,\"Cancelling\":null,\"CancellingTable\":null,\"Other\":null}",
                oldClrType: typeof(PlanMeta),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "meta",
                schema: "public",
                table: "facility",
                type: "json",
                nullable: true,
                defaultValue: "{\"RoomNumberWesternStyle\":null,\"RoomNumberJapaneseStyle\":null,\"RoomNumberJapaneseWesternStyle\":null,\"RoomNumberOtherStyle\":null,\"ReceptionLimit\":\"00:00:00\",\"CheckInStart\":\"00:00:00\",\"CheckInEnd\":\"00:00:00\",\"CheckOut\":\"00:00:00\",\"Heading1\":null,\"PRPointComment\":null,\"CancellingComment\":null,\"CancellingTable\":null,\"MapUrl\":null,\"UseSpaTax\":false,\"SpaTaxComment\":null,\"SpaTaxTable\":null,\"SpaType\":null,\"SpaName\":null,\"SpaDescription\":null,\"SpaInfoComment\":null,\"IsAcceptChildren\":false,\"AcceptChildrenInfoComment\":null,\"IsAcceptPet\":false,\"AcceptPetInfoComment\":null,\"IsBarrierFree\":false,\"BarrierFreeInfoComment\":null,\"MealTypeComment\":null,\"OnSidePaymentComment\":null,\"OnLinePaymentComment\":null,\"PaymentComment\":null,\"AccessInfoComment\":null,\"NearStationInfoComment\":null,\"ExistsParking\":false,\"ParkingInfoComment\":null,\"CanTransfer\":false,\"TransferComment\":null,\"EquipmentInfoComment\":null,\"RoomInfoComment\":null,\"AmenityInfoComment\":null,\"LeisureInfoComment\":null,\"FAQInfoComment\":null,\"OtherInfoComment\":null,\"UseWarnPrice\":false,\"WarnPrice\":0,\"UseWarnPricePercent\":false,\"WarnPricePercent\":0,\"OperationMode\":0,\"FileStorageLimit\":0.0,\"UseImportPlanName\":false,\"SEOCoefficient\":0.0,\"SystemEMail\":null}",
                oldClrType: typeof(FacilityMeta),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
