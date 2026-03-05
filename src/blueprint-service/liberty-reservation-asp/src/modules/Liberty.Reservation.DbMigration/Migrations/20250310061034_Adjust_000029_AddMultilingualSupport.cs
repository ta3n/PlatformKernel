using Liberty.Entity.Utils;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations;

/// <inheritdoc />
public partial class Adjust_000029_AddMultilingualSupport : Migration
{
    private readonly (string Schema, string Table, string Column) FacilityMeta = ("public", "facility", "meta");

    private readonly Dictionary<string, string> Properties = new()
    {
        { "Heading1", "heading1" },
        { "SpaTaxComment", "spa_tax_comment" },
        { "SpaTaxTable", "spa_tax_table" },
        { "BarrierFreeInfoComment", "barrier_free_info_comment" }
    };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        UpdateColumnsToJson(migrationBuilder);

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "facility",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

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
            oldDefaultValue: "{\"RoomNumberWesternStyle\":null,\"RoomNumberJapaneseStyle\":null,\"RoomNumberJapaneseWesternStyle\":null,\"RoomNumberOtherStyle\":null,\"ReceptionLimit\":\"00:00:00\",\"CheckInStart\":\"00:00:00\",\"CheckInEnd\":\"00:00:00\",\"CheckOut\":\"00:00:00\",\"Heading1\":null,\"PRPointComment\":null,\"CancellingComment\":null,\"CancellingTable\":null,\"MapUrl\":null,\"UseSpaTax\":false,\"SpaTaxComment\":null,\"SpaTaxTable\":null,\"SpaType\":null,\"SpaName\":null,\"SpaDescription\":null,\"SpaInfoComment\":null,\"IsAcceptChildren\":false,\"AcceptChildrenInfoComment\":null,\"IsAcceptPet\":false,\"AcceptPetInfoComment\":null,\"IsBarrierFree\":false,\"BarrierFreeInfoComment\":null,\"MealTypeComment\":null,\"OnSidePaymentComment\":null,\"OnLinePaymentComment\":null,\"PaymentComment\":null,\"AccessInfoComment\":null,\"NearStationInfoComment\":null,\"ExistsParking\":false,\"ParkingInfoComment\":null,\"CanTransfer\":false,\"TransferComment\":null,\"EquipmentInfoComment\":null,\"RoomInfoComment\":null,\"AmenityInfoComment\":null,\"LeisureInfoComment\":null,\"FAQInfoComment\":null,\"OtherInfoComment\":null,\"UseWarnPrice\":false,\"WarnPrice\":0,\"UseWarnPricePercent\":false,\"WarnPricePercent\":0,\"OperationMode\":0,\"FileStorageLimit\":0.0,\"UseImportPlanName\":false,\"SEOCoefficient\":0.0,\"SystemEMail\":null,\"Logo\":null}");

        migrationBuilder.AlterColumn<string>(
            name: "address4",
            schema: "public",
            table: "facility",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "address3",
            schema: "public",
            table: "facility",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "address2",
            schema: "public",
            table: "facility",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "address1",
            schema: "public",
            table: "facility",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "barrier_free_info_comment",
            schema: "public",
            table: "facility",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb");

        migrationBuilder.AddColumn<string>(
            name: "heading1",
            schema: "public",
            table: "facility",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb");

        migrationBuilder.AddColumn<string>(
            name: "spa_tax_comment",
            schema: "public",
            table: "facility",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb");

        migrationBuilder.AddColumn<string>(
            name: "spa_tax_table",
            schema: "public",
            table: "facility",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb");

        migrationBuilder.CreateIndex(
            name: "ix_facility_address1",
            schema: "public",
            table: "facility",
            column: "address1")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_facility_address2",
            schema: "public",
            table: "facility",
            column: "address2")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_facility_address3",
            schema: "public",
            table: "facility",
            column: "address3")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_facility_address4",
            schema: "public",
            table: "facility",
            column: "address4")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_facility_barrier_free_info_comment",
            schema: "public",
            table: "facility",
            column: "barrier_free_info_comment")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_facility_heading1",
            schema: "public",
            table: "facility",
            column: "heading1")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_facility_name",
            schema: "public",
            table: "facility",
            column: "name")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_facility_spa_tax_comment",
            schema: "public",
            table: "facility",
            column: "spa_tax_comment")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_facility_spa_tax_table",
            schema: "public",
            table: "facility",
            column: "spa_tax_table")
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
            defaultValue: "{\"RoomNumberWesternStyle\":null,\"RoomNumberJapaneseStyle\":null,\"RoomNumberJapaneseWesternStyle\":null,\"RoomNumberOtherStyle\":null,\"ReceptionLimit\":\"00:00:00\",\"CheckInStart\":\"00:00:00\",\"CheckInEnd\":\"00:00:00\",\"CheckOut\":\"00:00:00\",\"Heading1\":null,\"PRPointComment\":null,\"CancellingComment\":null,\"CancellingTable\":null,\"MapUrl\":null,\"UseSpaTax\":false,\"SpaTaxComment\":null,\"SpaTaxTable\":null,\"SpaType\":null,\"SpaName\":null,\"SpaDescription\":null,\"SpaInfoComment\":null,\"IsAcceptChildren\":false,\"AcceptChildrenInfoComment\":null,\"IsAcceptPet\":false,\"AcceptPetInfoComment\":null,\"IsBarrierFree\":false,\"BarrierFreeInfoComment\":null,\"MealTypeComment\":null,\"OnSidePaymentComment\":null,\"OnLinePaymentComment\":null,\"PaymentComment\":null,\"AccessInfoComment\":null,\"NearStationInfoComment\":null,\"ExistsParking\":false,\"ParkingInfoComment\":null,\"CanTransfer\":false,\"TransferComment\":null,\"EquipmentInfoComment\":null,\"RoomInfoComment\":null,\"AmenityInfoComment\":null,\"LeisureInfoComment\":null,\"FAQInfoComment\":null,\"OtherInfoComment\":null,\"UseWarnPrice\":false,\"WarnPrice\":0,\"UseWarnPricePercent\":false,\"WarnPricePercent\":0,\"OperationMode\":0,\"FileStorageLimit\":0.0,\"UseImportPlanName\":false,\"SEOCoefficient\":0.0,\"SystemEMail\":null,\"Logo\":null}",
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValue: "{\"RoomNumberWesternStyle\":null,\"RoomNumberJapaneseStyle\":null,\"RoomNumberJapaneseWesternStyle\":null,\"RoomNumberOtherStyle\":null,\"ReceptionLimit\":\"00:00:00\",\"CheckInStart\":\"00:00:00\",\"CheckInEnd\":\"00:00:00\",\"CheckOut\":\"00:00:00\",\"PRPointComment\":null,\"CancellingComment\":null,\"CancellingTable\":null,\"MapUrl\":null,\"UseSpaTax\":false,\"SpaType\":null,\"SpaName\":null,\"SpaDescription\":null,\"SpaInfoComment\":null,\"IsAcceptChildren\":false,\"AcceptChildrenInfoComment\":null,\"IsAcceptPet\":false,\"AcceptPetInfoComment\":null,\"IsBarrierFree\":false,\"MealTypeComment\":null,\"OnSidePaymentComment\":null,\"OnLinePaymentComment\":null,\"PaymentComment\":null,\"AccessInfoComment\":null,\"NearStationInfoComment\":null,\"ExistsParking\":false,\"ParkingInfoComment\":null,\"CanTransfer\":false,\"TransferComment\":null,\"EquipmentInfoComment\":null,\"RoomInfoComment\":null,\"AmenityInfoComment\":null,\"LeisureInfoComment\":null,\"FAQInfoComment\":null,\"OtherInfoComment\":null,\"UseWarnPrice\":false,\"WarnPrice\":0,\"UseWarnPricePercent\":false,\"WarnPricePercent\":0,\"OperationMode\":0,\"FileStorageLimit\":0.0,\"UseImportPlanName\":false,\"SEOCoefficient\":0.0,\"SystemEMail\":null,\"Logo\":null}");

        RevertMetaColumns(migrationBuilder);

        migrationBuilder.DropIndex(
            name: "ix_facility_address1",
            schema: "public",
            table: "facility");

        migrationBuilder.DropIndex(
            name: "ix_facility_address2",
            schema: "public",
            table: "facility");

        migrationBuilder.DropIndex(
            name: "ix_facility_address3",
            schema: "public",
            table: "facility");

        migrationBuilder.DropIndex(
            name: "ix_facility_address4",
            schema: "public",
            table: "facility");

        migrationBuilder.DropIndex(
            name: "ix_facility_barrier_free_info_comment",
            schema: "public",
            table: "facility");

        migrationBuilder.DropIndex(
            name: "ix_facility_heading1",
            schema: "public",
            table: "facility");

        migrationBuilder.DropIndex(
            name: "ix_facility_name",
            schema: "public",
            table: "facility");

        migrationBuilder.DropIndex(
            name: "ix_facility_spa_tax_comment",
            schema: "public",
            table: "facility");

        migrationBuilder.DropIndex(
            name: "ix_facility_spa_tax_table",
            schema: "public",
            table: "facility");

        migrationBuilder.DropColumn(
            name: "barrier_free_info_comment",
            schema: "public",
            table: "facility");

        migrationBuilder.DropColumn(
            name: "heading1",
            schema: "public",
            table: "facility");

        migrationBuilder.DropColumn(
            name: "spa_tax_comment",
            schema: "public",
            table: "facility");

        migrationBuilder.DropColumn(
            name: "spa_tax_table",
            schema: "public",
            table: "facility");

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "facility",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        migrationBuilder.AlterColumn<string>(
            name: "address4",
            schema: "public",
            table: "facility",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        migrationBuilder.AlterColumn<string>(
            name: "address3",
            schema: "public",
            table: "facility",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        migrationBuilder.AlterColumn<string>(
            name: "address2",
            schema: "public",
            table: "facility",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        migrationBuilder.AlterColumn<string>(
            name: "address1",
            schema: "public",
            table: "facility",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        RevertColumnsToJson(migrationBuilder);
    }

    private static IEnumerable<(string Schema, string Table, string Column)> GetColumnsToConvert()
    {
        yield return ("public", "facility", "name");
        yield return ("public", "facility", "address1");
        yield return ("public", "facility", "address2");
        yield return ("public", "facility", "address3");
        yield return ("public", "facility", "address4");
    }

    private static void UpdateColumnsToJson(MigrationBuilder migrationBuilder)
    {
        foreach (var (schema, table, column) in GetColumnsToConvert())
        {
            migrationBuilder.Sql(@$"
                    -- @formatter:off
                    UPDATE {schema}.{table}
                    SET {column} = jsonb_build_object('{LanguageHeaderUtil.DefaultLanguageCode}', {column})
                    WHERE {column} IS NOT NULL;
                    -- @formatter:on
                ");

            migrationBuilder.Sql(@$"
                    -- @formatter:off
                    ALTER TABLE {schema}.{table}
                    ALTER COLUMN {column} TYPE jsonb USING {column}::jsonb::jsonb;
                    -- @formatter:on
                ");
        }
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

    private static void RevertColumnsToJson(MigrationBuilder migrationBuilder)
    {
        foreach (var (schema, table, column) in GetColumnsToConvert())
        {
            migrationBuilder.Sql(@$"
                    -- @formatter:off
                    UPDATE {schema}.{table}
                    SET {column} = {column}::jsonb->>'{LanguageHeaderUtil.DefaultLanguageCode}'
                    WHERE {column} IS NOT NULL;
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
