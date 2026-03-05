# Implement Multilingual Support

## Table of Contents

- [Implement Multilingual Support](#implement-multilingual-support)
  - [Table of Contents](#table-of-contents)
  - [1. Create MultiLanguage Value Object](#1-create-multilanguage-value-object)
  - [2. Create Language Utility](#2-create-language-utility)
  - [3. Apply Multilingual Support to the Entity](#3-apply-multilingual-support-to-the-entity)
    - [3.1. Adjust the Entity](#31-adjust-the-entity)
    - [3.2. Create MultiLanguageConverter Method](#32-create-multilanguageconverter-method)
    - [3.3. Adjust the Entity Configuration](#33-adjust-the-entity-configuration)
  - [4. Create Database Migrations](#4-create-database-migrations)
    - [4.1. Convert Text Data to JSON](#41-convert-text-data-to-json)
    - [4.2. Apply Multilingual Support to the Database](#42-apply-multilingual-support-to-the-database)
    - [4.3. Create a migration to add multilingual support to the table](#43-create-a-migration-to-add-multilingual-support-to-the-table)
  - [5. Other Configurations](#5-other-configurations)
    - [5.1. AutoMapper Configuration](#51-automapper-configuration)
    - [5.2. Swagger UI Configuration](#52-swagger-ui-configuration)

## 1. Create MultiLanguage Value Object

```cs
public class MultiLanguage : Dictionary<string, string>
{
    public string GetValue(string languageCode)
    {
        return TryGetValue(languageCode, out var value) ? value : string.Empty;
    }

    public string GetValueByHeader()
    {
        var languageCode = LanguageHeaderUtil.GetLanguageCodeFromHeader();

        if (string.IsNullOrEmpty(languageCode))
        {
            languageCode = Keys.First();
        }

        return TryGetValue(languageCode, out var value) ? value : string.Empty;
    }

    public void UpdateLocalized(Dictionary<string, string>? updated, string? languageCode = null)
    {
        if (string.IsNullOrEmpty(languageCode))
        {
            languageCode = LanguageHeaderUtil.GetLanguageCodeFromHeader();
        }

        if (TryGetValue(languageCode, out _))
        {
            this[languageCode] = updated?[languageCode] ?? string.Empty;
        }
        else
        {
            Add(languageCode, updated?[languageCode] ?? string.Empty);
        }
    }
}
```

## 2. Create Language Utility

```cs
public static class LanguageHeaderUtil
{
    public const string DefaultLanguageCode = "ja";
    public const string DefaultAcceptLanguage = "ja-JP";

    public static string GetLanguageCodeFromHeader()
    {
        var context = new HttpContextAccessor().HttpContext;
        var acceptLanguage = context?.Request.Headers[HeaderNames.AcceptLanguage].ToString();
        var language = string.IsNullOrEmpty(acceptLanguage) ? DefaultAcceptLanguage : acceptLanguage.Split(',')[0];
        var languageCode = language.Split('-')[0];

        return languageCode;
    }
}
```

## 3. Apply Multilingual Support to the Entity

### 3.1. Adjust the Entity

```cs
public class Category : EntityData
{
    /// <summary>
    /// Category Type
    /// </summary>
    public CategoryTypes CategoryType { get; set; }

    /// <summary>
    /// Category Name
    /// </summary>
    public MultiLanguage? Name { get; set; }

    /// <summary>
    /// Category Description
    /// </summary>
    public MultiLanguage? Description { get; set; }

    public bool IsMaster { get; set; }

    [ForeignKey("Parent")]
    public long? ParentId { set; get; }

    public Category? Parent { set; get; }

    public ICollection<Category>? Children { set; get; }
}
```

### 3.2. Create MultiLanguageConverter Method

```cs
protected static ValueConverter<MultiLanguage?, string> MultiLanguageConverter()
{
    var multiLanguageConverter = new ValueConverter<MultiLanguage?, string>(
        x => x == null ? "null" : JsonConvert.SerializeObject(x), // Convert Dictionary -> JSON string
        x => x == "null" || string.IsNullOrEmpty(x)
            ? new()
            : JsonConvert.DeserializeObject<MultiLanguage>(x)!
    );

    return multiLanguageConverter;
}
```

### 3.3. Adjust the Entity Configuration

```cs
public class CategoryConfiguration : BaseDataEntityTypeConfiguration<Category>
{
    protected override void EntityConfigure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("category", DbConfiguration.DefaultSchema);

        builder.Property(e => e.Name)
            .HasColumnType("jsonb")
            .HasConversion(MultiLanguageConverter())
            .HasDefaultValueSql("'{}'::jsonb");

        builder
            .HasIndex(x => x.Name)
            .HasMethod("GIN");

        builder.Property(e => e.Description)
            .HasColumnType("jsonb")
            .HasConversion(MultiLanguageConverter())
            .HasDefaultValueSql("'{}'::jsonb");

        builder
            .HasIndex(x => x.Description)
            .HasMethod("GIN");
    }
}
```

## 4. Create Database Migrations

### 4.1. Convert Text Data to JSON

```cs
public partial class Adjust_convert_text_data_to_json : Migration
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
        }
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
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
}
```

### 4.2. Apply Multilingual Support to the Database

```cs
public partial class Adjust_multi_language_for_name_in_category_table : Migration
{
    private static List<(string schema, string TableName, string ColumnName)> GetColumnsToConvert()
    {
        return
        [
            ("public", "category", "name")
        ];
    }

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        foreach (var (schema, table, column) in GetColumnsToConvert())
        {
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
    }
}
```

- In PostgreSQL, when converting a column’s data type from string to jsonb, you need to include `USING {column}::jsonb::jsonb` in the `ALTER` statement.
- Currently, Entity Framework does not provide a built-in property to configure this conversion, so additional code is required to handle it.

### 4.3. Create a migration to add multilingual support to the table

```cs
public partial class Adjust_000023_AddMultilingualSupportToCategoryTable : Migration
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
```

## 5. Other Configurations

### 5.1. AutoMapper Configuration

```cs
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<MultiLanguage, string>().ConvertUsing(src => src == null ? string.Empty : src.GetValueByHeader());
    }
}
```

### 5.2. Swagger UI Configuration

```cs
public class AcceptLanguageSelectHeaderParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = HeaderNames.AcceptLanguage,
                In = ParameterLocation.Header,
                Description = "Accept-Language header",
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString(LanguageHeaderUtil.DefaultAcceptLanguage)
                },
                Required = true
            }
        );
    }
}
```

```cs
public static IServiceCollection AddSwaggerModule(this IServiceCollection services)
{
    services.AddSwaggerGen(
        options =>
        {
            options.OperationFilter<PageableModelFilter>();
            options.OperationFilter<AcceptLanguageSelectHeaderParameter>();

            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "Using the Authorization header with the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", securitySchema);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement { { securitySchema, ["Bearer"] } });
        }
    );
    services.ConfigureOptions<ConfigureSwaggerOptions>();
    services.ConfigureOptions<ConfigureSwaggerUiOptions>();

    return services;
}
```
