using Liberty.Entity.Utils;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000034_AddMultilingualSupport : Migration
    {
        private readonly (string Schema, string Table, string Column) FacilityMeta = ("public", "facility", "meta");

        private readonly Dictionary<string, string> Properties = new()
        {
            { "accessInfoComment", "access_info_comment" },
            { "nearStationInfoComment", "near_station_info_comment" },
            { "parkingInfoComment", "parking_info_comment" },
            { "transferComment", "transfer_comment" }
        };


        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "meta",
                schema: "public",
                table: "facility",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"RoomNumberWesternStyle\":null,\"RoomNumberJapaneseStyle\":null,\"RoomNumberJapaneseWesternStyle\":null,\"RoomNumberOtherStyle\":null,\"ReceptionLimit\":\"00:00:00\",\"CheckInStart\":\"00:00:00\",\"CheckInEnd\":\"00:00:00\",\"CheckOut\":\"00:00:00\",\"PRPointComment\":null,\"CancellingComment\":null,\"CancellingTable\":null,\"MapUrl\":null,\"UseSpaTax\":false,\"SpaType\":null,\"SpaName\":null,\"SpaDescription\":null,\"SpaInfoComment\":null,\"IsAcceptChildren\":false,\"AcceptChildrenInfoComment\":null,\"IsAcceptPet\":false,\"AcceptPetInfoComment\":null,\"IsBarrierFree\":false,\"MealTypeComment\":null,\"OnSidePaymentComment\":null,\"OnLinePaymentComment\":null,\"PaymentComment\":null,\"ExistsParking\":false,\"CanTransfer\":false,\"EquipmentInfoComment\":null,\"RoomInfoComment\":null,\"AmenityInfoComment\":null,\"LeisureInfoComment\":null,\"FAQInfoComment\":null,\"OtherInfoComment\":null,\"UseWarnPrice\":false,\"WarnPrice\":0,\"UseWarnPricePercent\":false,\"WarnPricePercent\":0,\"OperationMode\":0,\"FileStorageLimit\":0.0,\"UseImportPlanName\":false,\"SEOCoefficient\":0.0,\"SystemEMail\":null,\"Logo\":null}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"RoomNumberWesternStyle\":null,\"RoomNumberJapaneseStyle\":null,\"RoomNumberJapaneseWesternStyle\":null,\"RoomNumberOtherStyle\":null,\"ReceptionLimit\":\"00:00:00\",\"CheckInStart\":\"00:00:00\",\"CheckInEnd\":\"00:00:00\",\"CheckOut\":\"00:00:00\",\"PRPointComment\":null,\"CancellingComment\":null,\"CancellingTable\":null,\"MapUrl\":null,\"UseSpaTax\":false,\"SpaType\":null,\"SpaName\":null,\"SpaDescription\":null,\"SpaInfoComment\":null,\"IsAcceptChildren\":false,\"AcceptChildrenInfoComment\":null,\"IsAcceptPet\":false,\"AcceptPetInfoComment\":null,\"IsBarrierFree\":false,\"MealTypeComment\":null,\"OnSidePaymentComment\":null,\"OnLinePaymentComment\":null,\"PaymentComment\":null,\"AccessInfoComment\":null,\"NearStationInfoComment\":null,\"ExistsParking\":false,\"ParkingInfoComment\":null,\"CanTransfer\":false,\"TransferComment\":null,\"EquipmentInfoComment\":null,\"RoomInfoComment\":null,\"AmenityInfoComment\":null,\"LeisureInfoComment\":null,\"FAQInfoComment\":null,\"OtherInfoComment\":null,\"UseWarnPrice\":false,\"WarnPrice\":0,\"UseWarnPricePercent\":false,\"WarnPricePercent\":0,\"OperationMode\":0,\"FileStorageLimit\":0.0,\"UseImportPlanName\":false,\"SEOCoefficient\":0.0,\"SystemEMail\":null,\"Logo\":null}");

            migrationBuilder.AddColumn<string>(
                name: "access_info_comment",
                schema: "public",
                table: "facility",
                type: "jsonb",
                nullable: true,
                defaultValueSql: "'{}'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "near_station_info_comment",
                schema: "public",
                table: "facility",
                type: "jsonb",
                nullable: true,
                defaultValueSql: "'{}'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "parking_info_comment",
                schema: "public",
                table: "facility",
                type: "jsonb",
                nullable: true,
                defaultValueSql: "'{}'::jsonb");

            migrationBuilder.AddColumn<string>(
                name: "transfer_comment",
                schema: "public",
                table: "facility",
                type: "jsonb",
                nullable: true,
                defaultValueSql: "'{}'::jsonb");

            migrationBuilder.CreateIndex(
                name: "ix_facility_access_info_comment",
                schema: "public",
                table: "facility",
                column: "access_info_comment")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_facility_near_station_info_comment",
                schema: "public",
                table: "facility",
                column: "near_station_info_comment")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_facility_parking_info_comment",
                schema: "public",
                table: "facility",
                column: "parking_info_comment")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_facility_transfer_comment",
                schema: "public",
                table: "facility",
                column: "transfer_comment")
                .Annotation("Npgsql:IndexMethod", "GIN");

            UpdateMetaColumns(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "meta",
                schema: "public",
                table: "facility",
                type: "jsonb",
                nullable: true,
                defaultValue: "{\"RoomNumberWesternStyle\":null,\"RoomNumberJapaneseStyle\":null,\"RoomNumberJapaneseWesternStyle\":null,\"RoomNumberOtherStyle\":null,\"ReceptionLimit\":\"00:00:00\",\"CheckInStart\":\"00:00:00\",\"CheckInEnd\":\"00:00:00\",\"CheckOut\":\"00:00:00\",\"PRPointComment\":null,\"CancellingComment\":null,\"CancellingTable\":null,\"MapUrl\":null,\"UseSpaTax\":false,\"SpaType\":null,\"SpaName\":null,\"SpaDescription\":null,\"SpaInfoComment\":null,\"IsAcceptChildren\":false,\"AcceptChildrenInfoComment\":null,\"IsAcceptPet\":false,\"AcceptPetInfoComment\":null,\"IsBarrierFree\":false,\"MealTypeComment\":null,\"OnSidePaymentComment\":null,\"OnLinePaymentComment\":null,\"PaymentComment\":null,\"AccessInfoComment\":null,\"NearStationInfoComment\":null,\"ExistsParking\":false,\"ParkingInfoComment\":null,\"CanTransfer\":false,\"TransferComment\":null,\"EquipmentInfoComment\":null,\"RoomInfoComment\":null,\"AmenityInfoComment\":null,\"LeisureInfoComment\":null,\"FAQInfoComment\":null,\"OtherInfoComment\":null,\"UseWarnPrice\":false,\"WarnPrice\":0,\"UseWarnPricePercent\":false,\"WarnPricePercent\":0,\"OperationMode\":0,\"FileStorageLimit\":0.0,\"UseImportPlanName\":false,\"SEOCoefficient\":0.0,\"SystemEMail\":null,\"Logo\":null}",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValue: "{\"RoomNumberWesternStyle\":null,\"RoomNumberJapaneseStyle\":null,\"RoomNumberJapaneseWesternStyle\":null,\"RoomNumberOtherStyle\":null,\"ReceptionLimit\":\"00:00:00\",\"CheckInStart\":\"00:00:00\",\"CheckInEnd\":\"00:00:00\",\"CheckOut\":\"00:00:00\",\"PRPointComment\":null,\"CancellingComment\":null,\"CancellingTable\":null,\"MapUrl\":null,\"UseSpaTax\":false,\"SpaType\":null,\"SpaName\":null,\"SpaDescription\":null,\"SpaInfoComment\":null,\"IsAcceptChildren\":false,\"AcceptChildrenInfoComment\":null,\"IsAcceptPet\":false,\"AcceptPetInfoComment\":null,\"IsBarrierFree\":false,\"MealTypeComment\":null,\"OnSidePaymentComment\":null,\"OnLinePaymentComment\":null,\"PaymentComment\":null,\"ExistsParking\":false,\"CanTransfer\":false,\"EquipmentInfoComment\":null,\"RoomInfoComment\":null,\"AmenityInfoComment\":null,\"LeisureInfoComment\":null,\"FAQInfoComment\":null,\"OtherInfoComment\":null,\"UseWarnPrice\":false,\"WarnPrice\":0,\"UseWarnPricePercent\":false,\"WarnPricePercent\":0,\"OperationMode\":0,\"FileStorageLimit\":0.0,\"UseImportPlanName\":false,\"SEOCoefficient\":0.0,\"SystemEMail\":null,\"Logo\":null}");

            RevertMetaColumns(migrationBuilder);

            migrationBuilder.DropIndex(
                name: "ix_facility_access_info_comment",
                schema: "public",
                table: "facility");

            migrationBuilder.DropIndex(
                name: "ix_facility_near_station_info_comment",
                schema: "public",
                table: "facility");

            migrationBuilder.DropIndex(
                name: "ix_facility_parking_info_comment",
                schema: "public",
                table: "facility");

            migrationBuilder.DropIndex(
                name: "ix_facility_transfer_comment",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "access_info_comment",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "near_station_info_comment",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "parking_info_comment",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "transfer_comment",
                schema: "public",
                table: "facility");

        }
     
        private void UpdateMetaColumns(MigrationBuilder migrationBuilder)
        {
            var (schema, table, column) = FacilityMeta;

            foreach (var meta in Properties)
            {
                migrationBuilder.Sql(@$"
                    -- @formatter:off
                    UPDATE {schema}.{table}
                    SET {meta.Value} = jsonb_build_object('{LanguageHeaderUtil.DefaultLanguageCode}', {column}->>'{meta.Key}')
                    WHERE {column} IS NOT NULL AND {column}->>'{meta.Key}' IS NOT NULL;
                    -- @formatter:on
                ");
            }
        }

        private void RevertMetaColumns(MigrationBuilder migrationBuilder)
        {
            var (schema, table, column) = FacilityMeta;

            foreach (var meta in Properties)
            {
                migrationBuilder.Sql(@$"
                    -- @formatter:off
                    UPDATE {schema}.{table}
                    SET {column} = jsonb_set(
                        {column},
                        '{{{meta.Key}}}',
                        to_jsonb({meta.Value}->>'{LanguageHeaderUtil.DefaultLanguageCode}'),
                        true
                    )
                    WHERE {column} IS NOT NULL AND {meta.Value}->>'{LanguageHeaderUtil.DefaultLanguageCode}' IS NOT NULL;
                    -- @formatter:on
                ");
            }

        }
    }
}
