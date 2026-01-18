using Liberty.Entity.Utils;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations;

/// <inheritdoc />
public partial class Adjust_000023_AddMultilingualSupport : Migration
{
    private static List<(string schema, string TableName, string ColumnName)> GetColumnsToConvert()
    {
        return
        [
            ("public", "category", "name"),
            ("public", "category", "description"),
        ];
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
            table: "category",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "description",
            schema: "public",
            table: "category",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_category_description",
            schema: "public",
            table: "category",
            column: "description")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_category_name",
            schema: "public",
            table: "category",
            column: "name")
            .Annotation("Npgsql:IndexMethod", "GIN");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_category_description",
            schema: "public",
            table: "category");

        migrationBuilder.DropIndex(
            name: "ix_category_name",
            schema: "public",
            table: "category");

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "category",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        migrationBuilder.AlterColumn<string>(
            name: "description",
            schema: "public",
            table: "category",
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
