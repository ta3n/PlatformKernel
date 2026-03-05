using Liberty.Entity.Utils;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations;

/// <inheritdoc />
public partial class Adjust_000026_AddMultilingualSupport : Migration
{
    private static IEnumerable<(string Schema, string Table, string Column)> GetColumnsToConvert()
    {
        yield return ("public", "plan", "name");
        yield return ("public", "plan", "description");
        yield return ("public", "plan", "tag");
        yield return ("public", "plan", "name_for_import");
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
            name: "tag",
            schema: "public",
            table: "plan",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "name_for_import",
            schema: "public",
            table: "plan",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "plan",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "description",
            schema: "public",
            table: "plan",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_plan_description",
            schema: "public",
            table: "plan",
            column: "description")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_plan_name",
            schema: "public",
            table: "plan",
            column: "name")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_plan_name_for_import",
            schema: "public",
            table: "plan",
            column: "name_for_import")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_plan_tag",
            schema: "public",
            table: "plan",
            column: "tag")
            .Annotation("Npgsql:IndexMethod", "GIN");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_plan_description",
            schema: "public",
            table: "plan");

        migrationBuilder.DropIndex(
            name: "ix_plan_name",
            schema: "public",
            table: "plan");

        migrationBuilder.DropIndex(
            name: "ix_plan_name_for_import",
            schema: "public",
            table: "plan");

        migrationBuilder.DropIndex(
            name: "ix_plan_tag",
            schema: "public",
            table: "plan");

        migrationBuilder.AlterColumn<string>(
            name: "tag",
            schema: "public",
            table: "plan",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        migrationBuilder.AlterColumn<string>(
            name: "name_for_import",
            schema: "public",
            table: "plan",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "plan",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        migrationBuilder.AlterColumn<string>(
            name: "description",
            schema: "public",
            table: "plan",
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
