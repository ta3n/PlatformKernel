using Liberty.Entity.Utils;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations;

/// <inheritdoc />
public partial class Adjust_000027_AddMultilingualSupport : Migration
{
    private static IEnumerable<(string Schema, string Table, string Column)> GetColumnsToConvert()
    {
        yield return ("public", "person_age_type", "name");
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
            table: "person_age_type",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_person_age_type_name",
            schema: "public",
            table: "person_age_type",
            column: "name")
            .Annotation("Npgsql:IndexMethod", "GIN");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_person_age_type_name",
            schema: "public",
            table: "person_age_type");

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "person_age_type",
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
