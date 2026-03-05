using Liberty.Entity.Utils;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations;

/// <inheritdoc />
public partial class Adjust_000025_AddMultilingualSupport : Migration
{

    private static List<(string schema, string TableName, string ColumnName)> GetColumnsToConvert()
    {
        return
        [
             ("public", "cancellation", "name"),
             ("public", "cancellation", "description"),
             ("public", "cancellation_data", "description"),
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
            name: "description",
            schema: "public",
            table: "cancellation_data",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "cancellation",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "description",
            schema: "public",
            table: "cancellation",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb",
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "ix_cancellation_data_description",
            schema: "public",
            table: "cancellation_data",
            column: "description")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_cancellation_description",
            schema: "public",
            table: "cancellation",
            column: "description")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_cancellation_name",
            schema: "public",
            table: "cancellation",
            column: "name")
            .Annotation("Npgsql:IndexMethod", "GIN");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_cancellation_data_description",
            schema: "public",
            table: "cancellation_data");

        migrationBuilder.DropIndex(
            name: "ix_cancellation_description",
            schema: "public",
            table: "cancellation");

        migrationBuilder.DropIndex(
            name: "ix_cancellation_name",
            schema: "public",
            table: "cancellation");

        migrationBuilder.AlterColumn<string>(
            name: "description",
            schema: "public",
            table: "cancellation_data",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        migrationBuilder.AlterColumn<string>(
            name: "name",
            schema: "public",
            table: "cancellation",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValueSql: "'{}'::jsonb");

        migrationBuilder.AlterColumn<string>(
            name: "description",
            schema: "public",
            table: "cancellation",
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
