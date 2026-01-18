using Liberty.Entity.Utils;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations;

/// <inheritdoc />
public partial class Adjust_000030_AddMultilingualSupport : Migration
{
    private readonly (string Schema, string Table, string Column) PlanMeta = ("public", "plan", "meta");
    private readonly (string Schema, string Table, string Column) CancellationMeta = ("public", "cancellation", "meta");

    private readonly Dictionary<string, string> PlanProperties = new()
    {
        { "Meal", "meal" },
        { "Other", "other" },
        { "Payment", "payment" },
        { "Summary", "summary" }
    };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
         name: "table_source",
         schema: "public",
         table: "cancellation",
         type: "jsonb",
         nullable: true,
         defaultValueSql: "'{}'::jsonb");

        UpdateCancellationMeta(migrationBuilder);

        migrationBuilder.DropColumn(
            name: "meta",
            schema: "public",
            table: "cancellation");

        migrationBuilder.AlterColumn<string>(
            name: "meta",
            schema: "public",
            table: "plan",
            type: "jsonb",
            nullable: true,
            defaultValue: "{\"Heading1\":null,\"Description\":null,\"BarrierFree\":null,\"SpaTax\":null,\"SpaTaxTable\":null,\"Cancelling\":null,\"CancellingTable\":null}",
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValue: "{\"Heading1\":null,\"Summary\":null,\"Description\":null,\"Payment\":null,\"Meal\":null,\"BarrierFree\":null,\"SpaTax\":null,\"SpaTaxTable\":null,\"Cancelling\":null,\"CancellingTable\":null,\"Other\":null}");

        migrationBuilder.AddColumn<string>(
            name: "meal",
            schema: "public",
            table: "plan",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb");

        migrationBuilder.AddColumn<string>(
            name: "other",
            schema: "public",
            table: "plan",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb");

        migrationBuilder.AddColumn<string>(
            name: "payment",
            schema: "public",
            table: "plan",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb");

        migrationBuilder.AddColumn<string>(
            name: "summary",
            schema: "public",
            table: "plan",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'::jsonb");


        UpdatePlanMetaColumns(migrationBuilder);

        migrationBuilder.CreateIndex(
            name: "ix_plan_meal",
            schema: "public",
            table: "plan",
            column: "meal")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_plan_other",
            schema: "public",
            table: "plan",
            column: "other")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_plan_payment",
            schema: "public",
            table: "plan",
            column: "payment")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_plan_summary",
            schema: "public",
            table: "plan",
            column: "summary")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "ix_cancellation_table_source",
            schema: "public",
            table: "cancellation",
            column: "table_source")
            .Annotation("Npgsql:IndexMethod", "GIN");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "meta",
            schema: "public",
            table: "plan",
            type: "jsonb",
            nullable: true,
            defaultValue: "{\"Heading1\":null,\"Summary\":null,\"Description\":null,\"Payment\":null,\"Meal\":null,\"BarrierFree\":null,\"SpaTax\":null,\"SpaTaxTable\":null,\"Cancelling\":null,\"CancellingTable\":null,\"Other\":null}",
            oldClrType: typeof(string),
            oldType: "jsonb",
            oldNullable: true,
            oldDefaultValue: "{\"Heading1\":null,\"Description\":null,\"BarrierFree\":null,\"SpaTax\":null,\"SpaTaxTable\":null,\"Cancelling\":null,\"CancellingTable\":null}");

        RevertPlanMetaColumns(migrationBuilder);

        migrationBuilder.AddColumn<string>(
            name: "meta",
            schema: "public",
            table: "cancellation",
            type: "jsonb",
            nullable: true,
            defaultValue: "{\"TableSource\":null}");

        RevertCancellationMeta(migrationBuilder);

        migrationBuilder.DropIndex(
            name: "ix_plan_meal",
            schema: "public",
            table: "plan");

        migrationBuilder.DropIndex(
            name: "ix_plan_other",
            schema: "public",
            table: "plan");

        migrationBuilder.DropIndex(
            name: "ix_plan_payment",
            schema: "public",
            table: "plan");

        migrationBuilder.DropIndex(
            name: "ix_plan_summary",
            schema: "public",
            table: "plan");

        migrationBuilder.DropIndex(
            name: "ix_cancellation_table_source",
            schema: "public",
            table: "cancellation");

        migrationBuilder.DropColumn(
            name: "meal",
            schema: "public",
            table: "plan");

        migrationBuilder.DropColumn(
            name: "other",
            schema: "public",
            table: "plan");

        migrationBuilder.DropColumn(
            name: "payment",
            schema: "public",
            table: "plan");

        migrationBuilder.DropColumn(
            name: "summary",
            schema: "public",
            table: "plan");

        migrationBuilder.DropColumn(
            name: "table_source",
            schema: "public",
            table: "cancellation");
    }

    private void UpdatePlanMetaColumns(MigrationBuilder migrationBuilder)
    {
        foreach (var property in PlanProperties)
        {
            migrationBuilder.Sql($@"
                    UPDATE {PlanMeta.Schema}.{PlanMeta.Table}
                    SET {property.Value} = jsonb_build_object('{LanguageHeaderUtil.DefaultLanguageCode}', {PlanMeta.Column}->>'{property.Key}')
                    WHERE {PlanMeta.Column} IS NOT NULL AND {PlanMeta.Column}->>'{property.Key}' IS NOT NULL;
                ");
        }
    }

    private void UpdateCancellationMeta(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql($@"
                UPDATE {CancellationMeta.Schema}.{CancellationMeta.Table}
                SET table_source = jsonb_build_object('{LanguageHeaderUtil.DefaultLanguageCode}', {CancellationMeta.Column}->>'TableSource')
                WHERE {CancellationMeta.Column} IS NOT NULL AND {CancellationMeta.Column}->>'TableSource' IS NOT NULL;
            ");
    }

    private void RevertPlanMetaColumns(MigrationBuilder migrationBuilder)
    {
        foreach (var property in PlanProperties)
        {
            migrationBuilder.Sql($@"
                    UPDATE {PlanMeta.Schema}.{PlanMeta.Table}
                    SET {PlanMeta.Column} = jsonb_set(
                        {PlanMeta.Column},
                        '{{{property.Key}}}',
                        to_jsonb({property.Value}->>'{LanguageHeaderUtil.DefaultLanguageCode}'),
                        true
                    )
                    WHERE {PlanMeta.Column} IS NOT NULL AND {property.Value}->>'{LanguageHeaderUtil.DefaultLanguageCode}' IS NOT NULL;
                ");
        }
    }

    private void RevertCancellationMeta(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql($@"
                UPDATE {CancellationMeta.Schema}.{CancellationMeta.Table}
                SET {CancellationMeta.Column} = jsonb_set(
                    {CancellationMeta.Column},
                    '{{TableSource}}',
                    to_jsonb(table_source->>'{LanguageHeaderUtil.DefaultLanguageCode}'),
                    true
                )
                WHERE {CancellationMeta.Column} IS NOT NULL AND table_source->>'{LanguageHeaderUtil.DefaultLanguageCode}' IS NOT NULL;
            ");
    }

}
