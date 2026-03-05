using Liberty.Entity.Utils;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations;

/// <inheritdoc />
public partial class Adjust_000024_AddMultilingualSupport : Migration
{
    private static IEnumerable<(string Schema, string Table, string Column)> GetColumnsToConvert()
    {
        yield return ("public", "site", "name");
        yield return ("public", "room_group", "name");
        yield return ("public", "room_group", "description");
        yield return ("public", "option_item", "name");
    }

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
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

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "site",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "room_group",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "description",
            schema: "public",
            table: "room_group",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "option_item",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_site_name",
            schema: "public",
            table: "site",
            column: "name")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_room_group_description",
            schema: "public",
            table: "room_group",
            column: "description")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_room_group_name",
            schema: "public",
            table: "room_group",
            column: "name")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_option_item_name",
            schema: "public",
            table: "option_item",
            column: "name")
            .Annotation("Npgsql:IndexMethod", "GIN");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_site_name",
            schema: "public",
            table: "site");

        migrationBuilder.DropIndex(
            name: "ix_room_group_description",
            schema: "public",
            table: "room_group");

        migrationBuilder.DropIndex(
            name: "ix_room_group_name",
            schema: "public",
            table: "room_group");

        migrationBuilder.DropIndex(
            name: "ix_option_item_name",
            schema: "public",
            table: "option_item");

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "site",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "room_group",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        migrationBuilder.AlterColumn<string>(
            name: "description",
            schema: "public",
            table: "room_group",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "option_item",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

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
}
