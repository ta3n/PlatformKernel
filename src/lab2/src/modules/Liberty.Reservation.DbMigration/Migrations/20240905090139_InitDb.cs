using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "allergen",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_allergen", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "app_date",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_date", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "app_date_data",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    color = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_date_data", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "app_date_type",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    short_name = table.Column<string>(type: "text", nullable: true),
                    color = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_master = table.Column<bool>(type: "boolean", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_date_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "area",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    parent_id = table.Column<long>(type: "bigint", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_area", x => x.id);
                    table.ForeignKey(
                        name: "fk_area_area_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "public",
                        principalTable: "area",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "bed_type",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    bed_type_unit_type = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bed_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "calendar",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    event_memo = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_calendar", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cancellation",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    can_on_line_payment = table.Column<bool>(type: "boolean", nullable: false),
                    payment_limit = table.Column<int>(type: "integer", nullable: true),
                    meta_json = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cancellation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cancellation_data",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    day_start = table.Column<int>(type: "integer", nullable: false),
                    day_end = table.Column<int>(type: "integer", nullable: false),
                    rate = table.Column<float>(type: "real", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cancellation_data", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "category",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_type = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_master = table.Column<bool>(type: "boolean", nullable: false),
                    parent_id = table.Column<long>(type: "bigint", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category", x => x.id);
                    table.ForeignKey(
                        name: "fk_category_category_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "public",
                        principalTable: "category",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "consumption_tax",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    rate = table.Column<float>(type: "real", nullable: false),
                    enabled_start = table.Column<long>(type: "bigint", nullable: false),
                    enabled_end = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_consumption_tax", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "country",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    zone = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    english_name = table.Column<string>(type: "text", nullable: true),
                    lang_code = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_country", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "discount_data",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    start_prev_day = table.Column<int>(type: "integer", nullable: true),
                    end_prev_day = table.Column<int>(type: "integer", nullable: true),
                    person_min = table.Column<int>(type: "integer", nullable: true),
                    person_max = table.Column<int>(type: "integer", nullable: true),
                    value = table.Column<float>(type: "real", nullable: true),
                    price_setting_type = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_discount_data", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "fax_service",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    is_mail_fax = table.Column<bool>(type: "boolean", nullable: false),
                    mail_format = table.Column<string>(type: "text", nullable: true),
                    unit_price = table.Column<float>(type: "real", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fax_service", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "file",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    secret = table.Column<string>(type: "text", nullable: true),
                    extension = table.Column<string>(type: "text", nullable: true),
                    content_type = table.Column<string>(type: "text", nullable: true),
                    file_size = table.Column<float>(type: "real", nullable: false),
                    md5 = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    tag = table.Column<string>(type: "text", nullable: true),
                    meta_json = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gmo_payment_result_request",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    shop_id = table.Column<string>(type: "text", nullable: true),
                    shop_pass = table.Column<string>(type: "text", nullable: true),
                    access_id = table.Column<string>(type: "text", nullable: true),
                    access_pass = table.Column<string>(type: "text", nullable: true),
                    order_id = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    job_cd = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<string>(type: "text", nullable: true),
                    tax = table.Column<string>(type: "text", nullable: true),
                    currency = table.Column<string>(type: "text", nullable: true),
                    forward = table.Column<string>(type: "text", nullable: true),
                    method = table.Column<string>(type: "text", nullable: true),
                    pay_times = table.Column<string>(type: "text", nullable: true),
                    tran_id = table.Column<string>(type: "text", nullable: true),
                    approve = table.Column<string>(type: "text", nullable: true),
                    tran_date = table.Column<string>(type: "text", nullable: true),
                    err_code = table.Column<string>(type: "text", nullable: true),
                    err_info = table.Column<string>(type: "text", nullable: true),
                    pay_type = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_gmo_payment_result_request", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "login_history",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    meta_json = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_login_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "meal_type",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meal_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "option_item",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    base_number = table.Column<int>(type: "integer", nullable: true),
                    use_cancel_fee = table.Column<bool>(type: "boolean", nullable: false),
                    enabled_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    enabled_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    use_order_limit = table.Column<bool>(type: "boolean", nullable: false),
                    order_limit_before_day = table.Column<int>(type: "integer", nullable: true),
                    order_limit_before_day_span = table.Column<TimeSpan>(type: "interval", nullable: true),
                    use_auto_extend = table.Column<bool>(type: "boolean", nullable: false),
                    auto_extend_every_month_day = table.Column<int>(type: "integer", nullable: true),
                    auto_extend_month = table.Column<int>(type: "integer", nullable: true),
                    use_not_selled = table.Column<bool>(type: "boolean", nullable: false),
                    enabled_date_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    enabled_date_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    use_display_date = table.Column<bool>(type: "boolean", nullable: false),
                    display_date_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    display_date_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    use_accept_date = table.Column<bool>(type: "boolean", nullable: false),
                    accept_date_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    accept_date_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    meta_json = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_option_item", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    api_issue_code = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "person_age_type",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    is_master = table.Column<bool>(type: "boolean", nullable: false),
                    is_main = table.Column<bool>(type: "boolean", nullable: false),
                    age_max = table.Column<int>(type: "integer", nullable: true),
                    age_min = table.Column<int>(type: "integer", nullable: true),
                    meta_json = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_age_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "point",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    point_type = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expired_date = table.Column<long>(type: "bigint", nullable: false),
                    cancelled_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    enabled_start = table.Column<long>(type: "bigint", nullable: false),
                    expire = table.Column<int>(type: "integer", nullable: false),
                    parent_id = table.Column<long>(type: "bigint", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_point", x => x.id);
                    table.ForeignKey(
                        name: "fk_point_point_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "public",
                        principalTable: "point",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "point_rate",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    rate = table.Column<float>(type: "real", nullable: false),
                    expire = table.Column<int>(type: "integer", nullable: false),
                    enabled_start = table.Column<long>(type: "bigint", nullable: false),
                    enabled_end = table.Column<long>(type: "bigint", nullable: false),
                    point_rate_type = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_point_rate", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "prefecture",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_prefecture", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "price_data",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    person_min = table.Column<int>(type: "integer", nullable: true),
                    person_max = table.Column<int>(type: "integer", nullable: true),
                    price = table.Column<int>(type: "integer", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_price_data", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "que",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    que_type = table.Column<int>(type: "integer", nullable: false),
                    data = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_que", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "question",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    question_type = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    form_data = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "room",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    capacity_min = table.Column<int>(type: "integer", nullable: true),
                    capacity_max = table.Column<int>(type: "integer", nullable: true),
                    size = table.Column<float>(type: "real", nullable: true),
                    room_group_size_unit_type = table.Column<int>(type: "integer", nullable: false),
                    tag = table.Column<string>(type: "text", nullable: true),
                    meta_json = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "room_group",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    base_number = table.Column<int>(type: "integer", nullable: false),
                    group_name = table.Column<string>(type: "text", nullable: true),
                    capacity_min = table.Column<int>(type: "integer", nullable: true),
                    capacity_max = table.Column<int>(type: "integer", nullable: true),
                    size = table.Column<float>(type: "real", nullable: true),
                    room_group_size_unit_type = table.Column<int>(type: "integer", nullable: false),
                    tag = table.Column<string>(type: "text", nullable: true),
                    use_auto_extend = table.Column<bool>(type: "boolean", nullable: false),
                    auto_extend_every_month_day = table.Column<int>(type: "integer", nullable: true),
                    auto_extend_month = table.Column<int>(type: "integer", nullable: true),
                    enabled_date_start = table.Column<long>(type: "bigint", nullable: true),
                    enabled_date_end = table.Column<long>(type: "bigint", nullable: true),
                    is_enabled_smoking = table.Column<bool>(type: "boolean", nullable: false),
                    meta_json = table.Column<string>(type: "text", nullable: false),
                    is_auto_extend = table.Column<bool>(type: "boolean", nullable: false),
                    auto_extend_day_in_month = table.Column<int>(type: "integer", nullable: false),
                    auto_extend_after_month = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_group", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "site",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    short_name = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    memo = table.Column<string>(type: "text", nullable: true),
                    use_site_point = table.Column<bool>(type: "boolean", nullable: false),
                    is_master = table.Column<bool>(type: "boolean", nullable: false),
                    tag = table.Column<string>(type: "text", nullable: true),
                    meta = table.Column<string>(type: "json", nullable: true, defaultValue: "{\"RSSItemLinkFormat\":null}"),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_site", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "spa_tax_data",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    price_max = table.Column<int>(type: "integer", nullable: true),
                    price_min = table.Column<int>(type: "integer", nullable: true),
                    tax = table.Column<int>(type: "integer", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spa_tax_data", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "spa_tax_group",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    is_include_total_fee = table.Column<bool>(type: "boolean", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spa_tax_group", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "system_config",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    template_format_json = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_config", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "app_date_app_date_data",
                schema: "public",
                columns: table => new
                {
                    app_date_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_data_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_date_app_date_data", x => new { x.app_date_id, x.app_date_data_id });
                    table.ForeignKey(
                        name: "fk_app_date_app_date_data_app_date_app_date_id",
                        column: x => x.app_date_id,
                        principalSchema: "public",
                        principalTable: "app_date",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_app_date_app_date_data_app_date_data_app_date_data_id",
                        column: x => x.app_date_data_id,
                        principalSchema: "public",
                        principalTable: "app_date_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "app_date_app_date_type",
                schema: "public",
                columns: table => new
                {
                    app_date_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_type_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_date_app_date_type", x => new { x.app_date_id, x.app_date_type_id });
                    table.ForeignKey(
                        name: "fk_app_date_app_date_type_app_date_app_date_id",
                        column: x => x.app_date_id,
                        principalSchema: "public",
                        principalTable: "app_date",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_app_date_app_date_type_app_date_type_app_date_type_id",
                        column: x => x.app_date_type_id,
                        principalSchema: "public",
                        principalTable: "app_date_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "calendar_app_date_app_date_type",
                schema: "public",
                columns: table => new
                {
                    calendar_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_type_id = table.Column<long>(type: "bigint", nullable: false),
                    date_calendar = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_calendar_app_date_app_date_type", x => new { x.calendar_id, x.date_calendar, x.app_date_type_id });
                    table.ForeignKey(
                        name: "fk_calendar_app_date_app_date_type_app_date_type_app_date_type",
                        column: x => x.app_date_type_id,
                        principalSchema: "public",
                        principalTable: "app_date_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_calendar_app_date_app_date_type_calendar_calendar_id",
                        column: x => x.calendar_id,
                        principalSchema: "public",
                        principalTable: "calendar",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cancellation_cancellation_data",
                schema: "public",
                columns: table => new
                {
                    cancellation_id = table.Column<long>(type: "bigint", nullable: false),
                    cancellation_data_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cancellation_cancellation_data", x => new { x.cancellation_id, x.cancellation_data_id });
                    table.ForeignKey(
                        name: "fk_cancellation_cancellation_data_cancellation_cancellation_id",
                        column: x => x.cancellation_id,
                        principalSchema: "public",
                        principalTable: "cancellation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_cancellation_cancellation_data_cancellation_data_cancellati",
                        column: x => x.cancellation_data_id,
                        principalSchema: "public",
                        principalTable: "cancellation_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    kana = table.Column<string>(type: "text", nullable: true),
                    tag = table.Column<string>(type: "text", nullable: true),
                    area_id = table.Column<long>(type: "bigint", nullable: true),
                    category_id = table.Column<long>(type: "bigint", nullable: true),
                    country_id = table.Column<long>(type: "bigint", nullable: true),
                    post_code = table.Column<string>(type: "text", nullable: true),
                    address1 = table.Column<string>(type: "text", nullable: true),
                    address2 = table.Column<string>(type: "text", nullable: true),
                    address3 = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    fax = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    latitude = table.Column<float>(type: "real", nullable: false),
                    longitude = table.Column<float>(type: "real", nullable: false),
                    use_spa_tax_auto_calc = table.Column<bool>(type: "boolean", nullable: false),
                    is_on_side_payment = table.Column<bool>(type: "boolean", nullable: false),
                    is_on_line_payment = table.Column<bool>(type: "boolean", nullable: false),
                    can_on_line_payment = table.Column<bool>(type: "boolean", nullable: false),
                    cancel_limit_hour = table.Column<int>(type: "integer", nullable: false),
                    cancel_limit_minute = table.Column<int>(type: "integer", nullable: false),
                    meta = table.Column<string>(type: "json", nullable: true, defaultValue: "{\"RoomNumberWesternStyle\":null,\"RoomNumberJapaneseStyle\":null,\"RoomNumberJapaneseWesternStyle\":null,\"RoomNumberOtherStyle\":null,\"ReceptionLimit\":\"00:00:00\",\"CheckInStart\":\"00:00:00\",\"CheckInEnd\":\"00:00:00\",\"CheckOut\":\"00:00:00\",\"Heading1\":null,\"PRPointComment\":null,\"CancellingComment\":null,\"CancellingTable\":null,\"MapUrl\":null,\"UseSpaTax\":false,\"SpaTaxComment\":null,\"SpaTaxTable\":null,\"SpaType\":null,\"SpaName\":null,\"SpaDescription\":null,\"SpaInfoComment\":null,\"IsAcceptChildren\":false,\"AcceptChildrenInfoComment\":null,\"IsAcceptPet\":false,\"AcceptPetInfoComment\":null,\"IsBarrierFree\":false,\"BarrierFreeInfoComment\":null,\"MealTypeComment\":null,\"OnSidePaymentComment\":null,\"OnLinePaymentComment\":null,\"PaymentComment\":null,\"AccessInfoComment\":null,\"NearStationInfoComment\":null,\"ExistsParking\":false,\"ParkingInfoComment\":null,\"CanTransfer\":false,\"TransferComment\":null,\"EquipmentInfoComment\":null,\"RoomInfoComment\":null,\"AmenityInfoComment\":null,\"LeisureInfoComment\":null,\"FAQInfoComment\":null,\"OtherInfoComment\":null,\"UseWarnPrice\":false,\"WarnPrice\":0,\"UseWarnPricePercent\":false,\"WarnPricePercent\":0,\"OperationMode\":0,\"FileStorageLimit\":0.0,\"UseImportPlanName\":false,\"SEOCoefficient\":0.0,\"SystemEMail\":null}"),
                    memo = table.Column<string>(type: "text", nullable: true),
                    parent_id = table.Column<long>(type: "bigint", nullable: true),
                    use_daily_person = table.Column<bool>(type: "boolean", nullable: false),
                    can_add_room_on_modify = table.Column<bool>(type: "boolean", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility", x => x.id);
                    table.ForeignKey(
                        name: "fk_facility_area_area_id",
                        column: x => x.area_id,
                        principalSchema: "public",
                        principalTable: "area",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_facility_category_category_id",
                        column: x => x.category_id,
                        principalSchema: "public",
                        principalTable: "category",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_facility_country_country_id",
                        column: x => x.country_id,
                        principalSchema: "public",
                        principalTable: "country",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_facility_facility_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "file_category",
                schema: "public",
                columns: table => new
                {
                    file_id = table.Column<long>(type: "bigint", nullable: false),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_category", x => new { x.file_id, x.category_id });
                    table.ForeignKey(
                        name: "fk_file_category_category_category_id",
                        column: x => x.category_id,
                        principalSchema: "public",
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_file_category_file_file_id",
                        column: x => x.file_id,
                        principalSchema: "public",
                        principalTable: "file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "file_option_item",
                schema: "public",
                columns: table => new
                {
                    file_id = table.Column<long>(type: "bigint", nullable: false),
                    option_item_id = table.Column<long>(type: "bigint", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_option_item", x => new { x.file_id, x.option_item_id });
                    table.ForeignKey(
                        name: "fk_file_option_item_file_file_id",
                        column: x => x.file_id,
                        principalSchema: "public",
                        principalTable: "file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_file_option_item_option_item_option_item_id",
                        column: x => x.option_item_id,
                        principalSchema: "public",
                        principalTable: "option_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "option_item_app_date",
                schema: "public",
                columns: table => new
                {
                    option_item_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_id = table.Column<long>(type: "bigint", nullable: false),
                    is_not_selled = table.Column<bool>(type: "boolean", nullable: false),
                    sell_number = table.Column<int>(type: "integer", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_option_item_app_date", x => new { x.option_item_id, x.app_date_id });
                    table.ForeignKey(
                        name: "fk_option_item_app_date_app_date_app_date_id",
                        column: x => x.app_date_id,
                        principalSchema: "public",
                        principalTable: "app_date",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_option_item_app_date_option_item_option_item_id",
                        column: x => x.option_item_id,
                        principalSchema: "public",
                        principalTable: "option_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "option_item_category",
                schema: "public",
                columns: table => new
                {
                    option_item_id = table.Column<long>(type: "bigint", nullable: false),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_option_item_category", x => new { x.option_item_id, x.category_id });
                    table.ForeignKey(
                        name: "fk_option_item_category_category_category_id",
                        column: x => x.category_id,
                        principalSchema: "public",
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_option_item_category_option_item_option_item_id",
                        column: x => x.option_item_id,
                        principalSchema: "public",
                        principalTable: "option_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    name_for_import = table.Column<string>(type: "text", nullable: true),
                    enabled_date_start = table.Column<long>(type: "bigint", nullable: true),
                    enabled_date_end = table.Column<long>(type: "bigint", nullable: true),
                    use_display_date = table.Column<bool>(type: "boolean", nullable: false),
                    display_date_start = table.Column<long>(type: "bigint", nullable: true),
                    display_date_end = table.Column<long>(type: "bigint", nullable: true),
                    plan_type = table.Column<int>(type: "integer", nullable: false),
                    use_accept_date = table.Column<bool>(type: "boolean", nullable: false),
                    accept_date_start = table.Column<long>(type: "bigint", nullable: true),
                    accept_date_end = table.Column<long>(type: "bigint", nullable: true),
                    accept_months = table.Column<int>(type: "integer", nullable: true),
                    accept_end_limit_type = table.Column<int>(type: "integer", nullable: false),
                    accept_days = table.Column<int>(type: "integer", nullable: true),
                    use_day_sale_limit = table.Column<bool>(type: "boolean", nullable: false),
                    plan_day_sale_limit_type = table.Column<int>(type: "integer", nullable: false),
                    group_number_day_sale_limit = table.Column<int>(type: "integer", nullable: true),
                    room_number_day_sale_limit = table.Column<int>(type: "integer", nullable: true),
                    use_accept_person_number = table.Column<bool>(type: "boolean", nullable: false),
                    accept_person_number_min = table.Column<int>(type: "integer", nullable: true),
                    accept_person_number_max = table.Column<int>(type: "integer", nullable: true),
                    number_of_stay_limit_min = table.Column<int>(type: "integer", nullable: true),
                    number_of_stay_limit_max = table.Column<int>(type: "integer", nullable: true),
                    check_in_start = table.Column<TimeSpan>(type: "interval", nullable: true),
                    check_in_end = table.Column<TimeSpan>(type: "interval", nullable: true),
                    check_out = table.Column<TimeSpan>(type: "interval", nullable: true),
                    reception_day_limit = table.Column<int>(type: "integer", nullable: true),
                    reception_limit = table.Column<TimeSpan>(type: "interval", nullable: true),
                    is_cancel_same_accept = table.Column<bool>(type: "boolean", nullable: false),
                    cancel_day_limit = table.Column<int>(type: "integer", nullable: true),
                    cancel_limit = table.Column<TimeSpan>(type: "interval", nullable: true),
                    cancellation_id = table.Column<long>(type: "bigint", nullable: true),
                    is_on_side_payment = table.Column<bool>(type: "boolean", nullable: false),
                    is_on_line_payment = table.Column<bool>(type: "boolean", nullable: false),
                    tag = table.Column<string>(type: "text", nullable: true),
                    slag = table.Column<string>(type: "text", nullable: true),
                    is_secret = table.Column<bool>(type: "boolean", nullable: false),
                    secret_word = table.Column<string>(type: "text", nullable: true),
                    use_fixed_option_item = table.Column<bool>(type: "boolean", nullable: false),
                    use_optional_option_item = table.Column<bool>(type: "boolean", nullable: false),
                    point_rate_id = table.Column<long>(type: "bigint", nullable: true),
                    meta = table.Column<string>(type: "json", nullable: true, defaultValue: "{\"Heading1\":null,\"Summary\":null,\"Description\":null,\"Payment\":null,\"Meal\":null,\"BarrierFree\":null,\"SpaTax\":null,\"SpaTaxTable\":null,\"Cancelling\":null,\"CancellingTable\":null,\"Other\":null}"),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan", x => x.id);
                    table.ForeignKey(
                        name: "fk_plan_cancellation_cancellation_id",
                        column: x => x.cancellation_id,
                        principalSchema: "public",
                        principalTable: "cancellation",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_plan_point_rate_point_rate_id",
                        column: x => x.point_rate_id,
                        principalSchema: "public",
                        principalTable: "point_rate",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "option_item_question",
                schema: "public",
                columns: table => new
                {
                    option_item_id = table.Column<long>(type: "bigint", nullable: false),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_option_item_question", x => new { x.option_item_id, x.question_id });
                    table.ForeignKey(
                        name: "fk_option_item_question_option_item_option_item_id",
                        column: x => x.option_item_id,
                        principalSchema: "public",
                        principalTable: "option_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_option_item_question_question_question_id",
                        column: x => x.question_id,
                        principalSchema: "public",
                        principalTable: "question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "file_room",
                schema: "public",
                columns: table => new
                {
                    file_id = table.Column<long>(type: "bigint", nullable: false),
                    room_id = table.Column<long>(type: "bigint", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_room", x => new { x.file_id, x.room_id });
                    table.ForeignKey(
                        name: "fk_file_room_file_file_id",
                        column: x => x.file_id,
                        principalSchema: "public",
                        principalTable: "file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_file_room_room_room_id",
                        column: x => x.room_id,
                        principalSchema: "public",
                        principalTable: "room",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "file_room_group",
                schema: "public",
                columns: table => new
                {
                    file_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_room_group", x => new { x.file_id, x.room_group_id });
                    table.ForeignKey(
                        name: "fk_file_room_group_file_file_id",
                        column: x => x.file_id,
                        principalSchema: "public",
                        principalTable: "file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_file_room_group_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_group_app_date",
                schema: "public",
                columns: table => new
                {
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_id = table.Column<long>(type: "bigint", nullable: false),
                    is_not_selled = table.Column<bool>(type: "boolean", nullable: false),
                    sell_number = table.Column<int>(type: "integer", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_group_app_date", x => new { x.room_group_id, x.app_date_id });
                    table.ForeignKey(
                        name: "fk_room_group_app_date_app_date_app_date_id",
                        column: x => x.app_date_id,
                        principalSchema: "public",
                        principalTable: "app_date",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_app_date_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_group_app_date_type_price_data",
                schema: "public",
                columns: table => new
                {
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_type_id = table.Column<long>(type: "bigint", nullable: false),
                    price_data_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_group_app_date_type_price_data", x => new { x.room_group_id, x.app_date_type_id, x.price_data_id });
                    table.ForeignKey(
                        name: "fk_room_group_app_date_type_price_data_app_date_type_app_date_",
                        column: x => x.app_date_type_id,
                        principalSchema: "public",
                        principalTable: "app_date_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_app_date_type_price_data_price_data_price_data_id",
                        column: x => x.price_data_id,
                        principalSchema: "public",
                        principalTable: "price_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_app_date_type_price_data_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_group_bed_type",
                schema: "public",
                columns: table => new
                {
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    bed_type_id = table.Column<long>(type: "bigint", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_group_bed_type", x => new { x.room_group_id, x.bed_type_id });
                    table.ForeignKey(
                        name: "fk_room_group_bed_type_bed_type_bed_type_id",
                        column: x => x.bed_type_id,
                        principalSchema: "public",
                        principalTable: "bed_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_bed_type_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_group_category",
                schema: "public",
                columns: table => new
                {
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_group_category", x => new { x.room_group_id, x.category_id });
                    table.ForeignKey(
                        name: "fk_room_group_category_category_category_id",
                        column: x => x.category_id,
                        principalSchema: "public",
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_category_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_room_group",
                schema: "public",
                columns: table => new
                {
                    room_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_room_group", x => new { x.room_id, x.room_group_id });
                    table.ForeignKey(
                        name: "fk_room_room_group_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_room_group_room_room_id",
                        column: x => x.room_id,
                        principalSchema: "public",
                        principalTable: "room",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_group_site",
                schema: "public",
                columns: table => new
                {
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    use_auto_extend = table.Column<bool>(type: "boolean", nullable: false),
                    auto_extend_every_month_day = table.Column<int>(type: "integer", nullable: true),
                    auto_extend_month = table.Column<int>(type: "integer", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_group_site", x => new { x.room_group_id, x.site_id });
                    table.ForeignKey(
                        name: "fk_room_group_site_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_site_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_group_site_app_date",
                schema: "public",
                columns: table => new
                {
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_id = table.Column<long>(type: "bigint", nullable: false),
                    use_auto_discount = table.Column<bool>(type: "boolean", nullable: false),
                    point_rate = table.Column<float>(type: "real", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_group_site_app_date", x => new { x.room_group_id, x.site_id, x.app_date_id });
                    table.ForeignKey(
                        name: "fk_room_group_site_app_date_app_date_app_date_id",
                        column: x => x.app_date_id,
                        principalSchema: "public",
                        principalTable: "app_date",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_site_app_date_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_site_app_date_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_group_site_app_date_price_data",
                schema: "public",
                columns: table => new
                {
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_id = table.Column<long>(type: "bigint", nullable: false),
                    price_data_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_group_site_app_date_price_data", x => new { x.room_group_id, x.site_id, x.app_date_id, x.price_data_id });
                    table.ForeignKey(
                        name: "fk_room_group_site_app_date_price_data_app_date_app_date_id",
                        column: x => x.app_date_id,
                        principalSchema: "public",
                        principalTable: "app_date",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_site_app_date_price_data_price_data_price_data_id",
                        column: x => x.price_data_id,
                        principalSchema: "public",
                        principalTable: "price_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_site_app_date_price_data_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_site_app_date_price_data_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_group_site_app_date_type_price_data",
                schema: "public",
                columns: table => new
                {
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_type_id = table.Column<long>(type: "bigint", nullable: false),
                    price_data_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_group_site_app_date_type_price_data", x => new { x.room_group_id, x.site_id, x.app_date_type_id, x.price_data_id });
                    table.ForeignKey(
                        name: "fk_room_group_site_app_date_type_price_data_app_date_type_app_",
                        column: x => x.app_date_type_id,
                        principalSchema: "public",
                        principalTable: "app_date_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_site_app_date_type_price_data_price_data_price_d",
                        column: x => x.price_data_id,
                        principalSchema: "public",
                        principalTable: "price_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_site_app_date_type_price_data_room_group_room_gr",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_group_site_app_date_type_price_data_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "site_point_rate",
                schema: "public",
                columns: table => new
                {
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    point_rate_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_site_point_rate", x => new { x.site_id, x.point_rate_id });
                    table.ForeignKey(
                        name: "fk_site_point_rate_point_rate_point_rate_id",
                        column: x => x.point_rate_id,
                        principalSchema: "public",
                        principalTable: "point_rate",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_site_point_rate_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "person_age_type_spa_tax_data",
                schema: "public",
                columns: table => new
                {
                    person_age_type_id = table.Column<long>(type: "bigint", nullable: false),
                    spa_tax_data_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_age_type_spa_tax_data", x => new { x.person_age_type_id, x.spa_tax_data_id });
                    table.ForeignKey(
                        name: "fk_person_age_type_spa_tax_data_person_age_type_person_age_typ",
                        column: x => x.person_age_type_id,
                        principalSchema: "public",
                        principalTable: "person_age_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_person_age_type_spa_tax_data_spa_tax_data_spa_tax_data_id",
                        column: x => x.spa_tax_data_id,
                        principalSchema: "public",
                        principalTable: "spa_tax_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "spa_tax_group_spa_tax_data",
                schema: "public",
                columns: table => new
                {
                    spa_tax_group_id = table.Column<long>(type: "bigint", nullable: false),
                    spa_tax_data_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spa_tax_group_spa_tax_data", x => new { x.spa_tax_group_id, x.spa_tax_data_id });
                    table.ForeignKey(
                        name: "fk_spa_tax_group_spa_tax_data_spa_tax_data_spa_tax_data_id",
                        column: x => x.spa_tax_data_id,
                        principalSchema: "public",
                        principalTable: "spa_tax_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_spa_tax_group_spa_tax_data_spa_tax_group_spa_tax_group_id",
                        column: x => x.spa_tax_group_id,
                        principalSchema: "public",
                        principalTable: "spa_tax_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_allergen",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    allergen_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_allergen", x => new { x.facility_id, x.allergen_id });
                    table.ForeignKey(
                        name: "fk_facility_allergen_allergen_allergen_id",
                        column: x => x.allergen_id,
                        principalSchema: "public",
                        principalTable: "allergen",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_allergen_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_app_date_type",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_type_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_app_date_type", x => new { x.facility_id, x.app_date_type_id });
                    table.ForeignKey(
                        name: "fk_facility_app_date_type_app_date_type_app_date_type_id",
                        column: x => x.app_date_type_id,
                        principalSchema: "public",
                        principalTable: "app_date_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_app_date_type_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_calendar",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    calendar_id = table.Column<long>(type: "bigint", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    enabled_start = table.Column<long>(type: "bigint", nullable: true),
                    enabled_end = table.Column<long>(type: "bigint", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_calendar", x => new { x.facility_id, x.calendar_id });
                    table.ForeignKey(
                        name: "fk_facility_calendar_calendar_calendar_id",
                        column: x => x.calendar_id,
                        principalSchema: "public",
                        principalTable: "calendar",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_calendar_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_cancellation",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    cancellation_id = table.Column<long>(type: "bigint", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_cancellation", x => new { x.facility_id, x.cancellation_id });
                    table.ForeignKey(
                        name: "fk_facility_cancellation_cancellation_cancellation_id",
                        column: x => x.cancellation_id,
                        principalSchema: "public",
                        principalTable: "cancellation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_cancellation_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_category",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_category", x => new { x.facility_id, x.category_id });
                    table.ForeignKey(
                        name: "fk_facility_category_category_category_id",
                        column: x => x.category_id,
                        principalSchema: "public",
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_category_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_fax_service",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    fax_service_id = table.Column<long>(type: "bigint", nullable: false),
                    enabled_start = table.Column<long>(type: "bigint", nullable: true),
                    enabled_end = table.Column<long>(type: "bigint", nullable: true),
                    claim_action_price = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<float>(type: "real", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_fax_service", x => new { x.facility_id, x.fax_service_id });
                    table.ForeignKey(
                        name: "fk_facility_fax_service_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_fax_service_fax_service_fax_service_id",
                        column: x => x.fax_service_id,
                        principalSchema: "public",
                        principalTable: "fax_service",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_file",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    file_id = table.Column<long>(type: "bigint", nullable: false),
                    file_purpose_type = table.Column<int>(type: "integer", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_file", x => new { x.facility_id, x.file_id });
                    table.ForeignKey(
                        name: "fk_facility_file_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_file_file_file_id",
                        column: x => x.file_id,
                        principalSchema: "public",
                        principalTable: "file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_option_item",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    option_item_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_option_item", x => new { x.facility_id, x.option_item_id });
                    table.ForeignKey(
                        name: "fk_facility_option_item_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_option_item_option_item_option_item_id",
                        column: x => x.option_item_id,
                        principalSchema: "public",
                        principalTable: "option_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_person_age_type",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    person_age_type_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_person_age_type", x => new { x.facility_id, x.person_age_type_id });
                    table.ForeignKey(
                        name: "fk_facility_person_age_type_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_person_age_type_person_age_type_person_age_type_id",
                        column: x => x.person_age_type_id,
                        principalSchema: "public",
                        principalTable: "person_age_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_question",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_question", x => new { x.facility_id, x.question_id });
                    table.ForeignKey(
                        name: "fk_facility_question_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_question_question_question_id",
                        column: x => x.question_id,
                        principalSchema: "public",
                        principalTable: "question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_room_group",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_room_group", x => new { x.facility_id, x.room_group_id });
                    table.ForeignKey(
                        name: "fk_facility_room_group_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_room_group_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_site",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_site", x => new { x.facility_id, x.site_id });
                    table.ForeignKey(
                        name: "fk_facility_site_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_site_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_spa_tax_group",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    spa_tax_group_id = table.Column<long>(type: "bigint", nullable: false),
                    enabled_start = table.Column<long>(type: "bigint", nullable: true),
                    enabled_end = table.Column<long>(type: "bigint", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_spa_tax_group", x => new { x.facility_id, x.spa_tax_group_id });
                    table.ForeignKey(
                        name: "fk_facility_spa_tax_group_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_spa_tax_group_spa_tax_group_spa_tax_group_id",
                        column: x => x.spa_tax_group_id,
                        principalSchema: "public",
                        principalTable: "spa_tax_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_plan",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_plan", x => new { x.facility_id, x.plan_id });
                    table.ForeignKey(
                        name: "fk_facility_plan_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_facility_plan_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "favorite",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plan_id = table.Column<long>(type: "bigint", nullable: true),
                    facility_id = table.Column<long>(type: "bigint", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_favorite", x => x.id);
                    table.ForeignKey(
                        name: "fk_favorite_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_favorite_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "file_plan",
                schema: "public",
                columns: table => new
                {
                    file_id = table.Column<long>(type: "bigint", nullable: false),
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_plan", x => new { x.file_id, x.plan_id });
                    table.ForeignKey(
                        name: "fk_file_plan_file_file_id",
                        column: x => x.file_id,
                        principalSchema: "public",
                        principalTable: "file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_file_plan_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_category",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_category", x => new { x.plan_id, x.category_id });
                    table.ForeignKey(
                        name: "fk_plan_category_category_category_id",
                        column: x => x.category_id,
                        principalSchema: "public",
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_category_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_meal_type",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    meal_type_id = table.Column<long>(type: "bigint", nullable: false),
                    meal_type_eat_type = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_meal_type", x => new { x.plan_id, x.meal_type_id });
                    table.ForeignKey(
                        name: "fk_plan_meal_type_meal_type_meal_type_id",
                        column: x => x.meal_type_id,
                        principalSchema: "public",
                        principalTable: "meal_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_meal_type_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_option_item",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    option_item_id = table.Column<long>(type: "bigint", nullable: false),
                    plan_option_item_type = table.Column<int>(type: "integer", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: true),
                    option_item_target = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_option_item", x => new { x.plan_id, x.option_item_id, x.plan_option_item_type });
                    table.ForeignKey(
                        name: "fk_plan_option_item_option_item_option_item_id",
                        column: x => x.option_item_id,
                        principalSchema: "public",
                        principalTable: "option_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_option_item_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_question",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_question", x => new { x.plan_id, x.question_id });
                    table.ForeignKey(
                        name: "fk_plan_question_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_question_question_question_id",
                        column: x => x.question_id,
                        principalSchema: "public",
                        principalTable: "question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_room_group",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_room_group", x => new { x.plan_id, x.room_group_id });
                    table.ForeignKey(
                        name: "fk_plan_room_group_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_room_group_cancellation",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    cancellation_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_room_group_cancellation", x => new { x.plan_id, x.room_group_id, x.cancellation_id });
                    table.ForeignKey(
                        name: "fk_plan_room_group_cancellation_cancellation_cancellation_id",
                        column: x => x.cancellation_id,
                        principalSchema: "public",
                        principalTable: "cancellation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_cancellation_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_cancellation_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_room_group_site_app_date",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    date_calendar = table.Column<long>(type: "bigint", nullable: false),
                    use_auto_discount = table.Column<bool>(type: "boolean", nullable: false),
                    point_rate = table.Column<float>(type: "real", nullable: true),
                    app_date_id = table.Column<long>(type: "bigint", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_room_group_site_app_date", x => new { x.plan_id, x.room_group_id, x.site_id, x.date_calendar });
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_app_date_app_date_id",
                        column: x => x.app_date_id,
                        principalSchema: "public",
                        principalTable: "app_date",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_room_group_site_app_date_price_data",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    date_calendar = table.Column<long>(type: "bigint", nullable: false),
                    price_data_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_id = table.Column<long>(type: "bigint", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_room_group_site_app_date_price_data", x => new { x.plan_id, x.room_group_id, x.site_id, x.date_calendar, x.price_data_id });
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_price_data_app_date_app_date_",
                        column: x => x.app_date_id,
                        principalSchema: "public",
                        principalTable: "app_date",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_price_data_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_price_data_price_data_price_d",
                        column: x => x.price_data_id,
                        principalSchema: "public",
                        principalTable: "price_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_price_data_room_group_room_gr",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_price_data_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_room_group_site_app_date_type_price_data",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_type_id = table.Column<long>(type: "bigint", nullable: false),
                    price_data_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_room_group_site_app_date_type_price_data", x => new { x.plan_id, x.room_group_id, x.site_id, x.app_date_type_id, x.price_data_id });
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_type_price_data_app_date_type",
                        column: x => x.app_date_type_id,
                        principalSchema: "public",
                        principalTable: "app_date_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_type_price_data_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_type_price_data_price_data_pr",
                        column: x => x.price_data_id,
                        principalSchema: "public",
                        principalTable: "price_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_type_price_data_room_group_ro",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_app_date_type_price_data_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_room_group_site_discount_data",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    discount_data_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_room_group_site_discount_data", x => new { x.plan_id, x.room_group_id, x.site_id, x.discount_data_id });
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_discount_data_discount_data_discount_d",
                        column: x => x.discount_data_id,
                        principalSchema: "public",
                        principalTable: "discount_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_discount_data_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_discount_data_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_discount_data_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_room_group_site_person_age_type",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    person_age_type_id = table.Column<long>(type: "bigint", nullable: false),
                    is_regard_adult = table.Column<bool>(type: "boolean", nullable: false),
                    price_setting_type = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<float>(type: "real", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_room_group_site_person_age_type", x => new { x.plan_id, x.room_group_id, x.site_id, x.person_age_type_id });
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_person_age_type_person_age_type_person",
                        column: x => x.person_age_type_id,
                        principalSchema: "public",
                        principalTable: "person_age_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_person_age_type_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_person_age_type_room_group_room_group_",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_person_age_type_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_site",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_site", x => new { x.plan_id, x.site_id });
                    table.ForeignKey(
                        name: "fk_plan_site_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_site_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_room_group_site",
                schema: "public",
                columns: table => new
                {
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    site_id = table.Column<long>(type: "bigint", nullable: false),
                    use_auto_extend = table.Column<bool>(type: "boolean", nullable: false),
                    auto_extend_every_month_day = table.Column<int>(type: "integer", nullable: true),
                    auto_extend_month = table.Column<int>(type: "integer", nullable: true),
                    plan_room_group_plan_id = table.Column<long>(type: "bigint", nullable: true),
                    plan_room_group_room_group_id = table.Column<long>(type: "bigint", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_room_group_site", x => new { x.plan_id, x.room_group_id, x.site_id });
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_plan_room_group_plan_room_group_plan_i",
                        columns: x => new { x.plan_room_group_plan_id, x.plan_room_group_room_group_id },
                        principalSchema: "public",
                        principalTable: "plan_room_group",
                        principalColumns: new[] { "plan_id", "room_group_id" });
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plan_room_group_site_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_user_favorite",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    favorite_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_user_favorite", x => new { x.user_id, x.favorite_id });
                    table.ForeignKey(
                        name: "fk_application_user_favorite_favorite_favorite_id",
                        column: x => x.favorite_id,
                        principalSchema: "public",
                        principalTable: "favorite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_user_login_history",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    login_history_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_user_login_history", x => new { x.user_id, x.login_history_id });
                    table.ForeignKey(
                        name: "fk_application_user_login_history_login_history_login_history_",
                        column: x => x.login_history_id,
                        principalSchema: "public",
                        principalTable: "login_history",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_user_point",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    point_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_user_point", x => new { x.user_id, x.point_id });
                    table.ForeignKey(
                        name: "fk_application_user_point_point_point_id",
                        column: x => x.point_id,
                        principalSchema: "public",
                        principalTable: "point",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_application_user",
                schema: "public",
                columns: table => new
                {
                    facility_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facility_application_user", x => new { x.facility_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_facility_application_user_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_gmo_payment_result_request",
                schema: "public",
                columns: table => new
                {
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    gmo_payment_result_request_id = table.Column<long>(type: "bigint", nullable: false),
                    valid_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reservation_id = table.Column<long>(type: "bigint", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_gmo_payment_result_request", x => new { x.order_id, x.gmo_payment_result_request_id });
                    table.ForeignKey(
                        name: "fk_order_gmo_payment_result_request_gmo_payment_result_request",
                        column: x => x.gmo_payment_result_request_id,
                        principalSchema: "public",
                        principalTable: "gmo_payment_result_request",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_gmo_payment_result_request_order_order_id",
                        column: x => x.order_id,
                        principalSchema: "public",
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_reservation",
                schema: "public",
                columns: table => new
                {
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    reservation_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_reservation", x => new { x.order_id, x.reservation_id });
                    table.ForeignKey(
                        name: "fk_order_reservation_order_order_id",
                        column: x => x.order_id,
                        principalSchema: "public",
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservation",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    serial = table.Column<string>(type: "text", nullable: true),
                    facility_id = table.Column<long>(type: "bigint", nullable: true),
                    plan_id = table.Column<long>(type: "bigint", nullable: true),
                    room_group_id = table.Column<long>(type: "bigint", nullable: true),
                    site_id = table.Column<long>(type: "bigint", nullable: true),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    reserver_id = table.Column<long>(type: "bigint", nullable: true),
                    main_user_id = table.Column<long>(type: "bigint", nullable: true),
                    check_in_date = table.Column<long>(type: "bigint", nullable: false),
                    rest_number = table.Column<int>(type: "integer", nullable: false),
                    room_number = table.Column<int>(type: "integer", nullable: false),
                    check_in_time = table.Column<TimeSpan>(type: "interval", nullable: true),
                    check_out_time = table.Column<TimeSpan>(type: "interval", nullable: true),
                    payment_type = table.Column<int>(type: "integer", nullable: false),
                    used_point = table.Column<int>(type: "integer", nullable: false),
                    income_point = table.Column<int>(type: "integer", nullable: false),
                    reservation_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    confirmed_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reservation_state = table.Column<int>(type: "integer", nullable: false),
                    modified_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelled_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancellation_price = table.Column<int>(type: "integer", nullable: true),
                    no_show_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    no_show_reason = table.Column<string>(type: "text", nullable: true),
                    parent_id = table.Column<long>(type: "bigint", nullable: true),
                    memo = table.Column<string>(type: "text", nullable: true),
                    json_data = table.Column<string>(type: "text", nullable: false),
                    is_same_main_user = table.Column<bool>(type: "boolean", nullable: false),
                    use_room_user = table.Column<bool>(type: "boolean", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation", x => x.id);
                    table.ForeignKey(
                        name: "fk_reservation_facility_facility_id",
                        column: x => x.facility_id,
                        principalSchema: "public",
                        principalTable: "facility",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_reservation_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_reservation_reservation_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "public",
                        principalTable: "reservation",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_reservation_room_group_room_group_id",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_reservation_site_site_id",
                        column: x => x.site_id,
                        principalSchema: "public",
                        principalTable: "site",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "reservation_point",
                schema: "public",
                columns: table => new
                {
                    reservation_id = table.Column<long>(type: "bigint", nullable: false),
                    point_id = table.Column<long>(type: "bigint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation_point", x => new { x.reservation_id, x.point_id });
                    table.ForeignKey(
                        name: "fk_reservation_point_point_point_id",
                        column: x => x.point_id,
                        principalSchema: "public",
                        principalTable: "point",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_point_reservation_reservation_id",
                        column: x => x.reservation_id,
                        principalSchema: "public",
                        principalTable: "reservation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservation_question",
                schema: "public",
                columns: table => new
                {
                    reservation_id = table.Column<long>(type: "bigint", nullable: false),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    reservation_question_type = table.Column<int>(type: "integer", nullable: false),
                    question_obj = table.Column<string>(type: "text", nullable: true),
                    answer_data = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation_question", x => new { x.reservation_id, x.question_id, x.reservation_question_type });
                    table.ForeignKey(
                        name: "fk_reservation_question_question_question_id",
                        column: x => x.question_id,
                        principalSchema: "public",
                        principalTable: "question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_question_reservation_reservation_id",
                        column: x => x.reservation_id,
                        principalSchema: "public",
                        principalTable: "reservation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservation_room_group_app_date_person_age_type",
                schema: "public",
                columns: table => new
                {
                    reservation_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_id = table.Column<long>(type: "bigint", nullable: false),
                    person_age_type_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_index = table.Column<int>(type: "integer", nullable: false),
                    rest_index = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<int>(type: "integer", nullable: false),
                    spa_tax = table.Column<int>(type: "integer", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation_room_group_app_date_person_age_type", x => new { x.reservation_id, x.room_group_id, x.app_date_id, x.person_age_type_id, x.room_group_index });
                    table.ForeignKey(
                        name: "fk_reservation_room_group_app_date_person_age_type_app_date_ap",
                        column: x => x.app_date_id,
                        principalSchema: "public",
                        principalTable: "app_date",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_room_group_app_date_person_age_type_person_age_",
                        column: x => x.person_age_type_id,
                        principalSchema: "public",
                        principalTable: "person_age_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_room_group_app_date_person_age_type_reservation",
                        column: x => x.reservation_id,
                        principalSchema: "public",
                        principalTable: "reservation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_room_group_app_date_person_age_type_room_group_",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_info",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    kana = table.Column<string>(type: "text", nullable: true),
                    e_mail = table.Column<string>(type: "text", nullable: true),
                    name_e = table.Column<string>(type: "text", nullable: true),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    country_id = table.Column<long>(type: "bigint", nullable: true),
                    post_code = table.Column<string>(type: "text", nullable: true),
                    address1 = table.Column<string>(type: "text", nullable: true),
                    address2 = table.Column<string>(type: "text", nullable: true),
                    address3 = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    birth_day = table.Column<long>(type: "bigint", nullable: true),
                    reservation_room_group_app_date_person_age_type_app_date_id = table.Column<long>(type: "bigint", nullable: true),
                    reservation_room_group_app_date_person_age_type_person_age_type_id = table.Column<long>(type: "bigint", nullable: true),
                    reservation_room_group_app_date_person_age_type_reservation_id = table.Column<long>(type: "bigint", nullable: true),
                    reservation_room_group_app_date_person_age_type_room_group_id = table.Column<long>(type: "bigint", nullable: true),
                    reservation_room_group_app_date_person_age_type_room_group_index = table.Column<int>(type: "integer", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_info", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_info_country_country_id",
                        column: x => x.country_id,
                        principalSchema: "public",
                        principalTable: "country",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_user_info_reservation_room_group_app_date_person_age_type_r",
                        columns: x => new { x.reservation_room_group_app_date_person_age_type_reservation_id, x.reservation_room_group_app_date_person_age_type_room_group_id, x.reservation_room_group_app_date_person_age_type_app_date_id, x.reservation_room_group_app_date_person_age_type_person_age_type_id, x.reservation_room_group_app_date_person_age_type_room_group_index },
                        principalSchema: "public",
                        principalTable: "reservation_room_group_app_date_person_age_type",
                        principalColumns: new[] { "reservation_id", "room_group_id", "app_date_id", "person_age_type_id", "room_group_index" });
                });

            migrationBuilder.CreateTable(
                name: "reservation_plan_room_group_app_date",
                schema: "public",
                columns: table => new
                {
                    reservation_id = table.Column<long>(type: "bigint", nullable: false),
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_index = table.Column<int>(type: "integer", nullable: false),
                    rest_index = table.Column<int>(type: "integer", nullable: false),
                    user_info_id = table.Column<long>(type: "bigint", nullable: true),
                    check_in_time = table.Column<TimeSpan>(type: "interval", nullable: true),
                    check_out_time = table.Column<TimeSpan>(type: "interval", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation_plan_room_group_app_date", x => new { x.reservation_id, x.plan_id, x.room_group_id, x.app_date_id, x.room_group_index });
                    table.ForeignKey(
                        name: "fk_reservation_plan_room_group_app_date_app_date_app_date_id",
                        column: x => x.app_date_id,
                        principalSchema: "public",
                        principalTable: "app_date",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_plan_room_group_app_date_plan_plan_id",
                        column: x => x.plan_id,
                        principalSchema: "public",
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_plan_room_group_app_date_plan_room_group_plan_i",
                        columns: x => new { x.plan_id, x.room_group_id },
                        principalSchema: "public",
                        principalTable: "plan_room_group",
                        principalColumns: new[] { "plan_id", "room_group_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_plan_room_group_app_date_reservation_reservatio",
                        column: x => x.reservation_id,
                        principalSchema: "public",
                        principalTable: "reservation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_plan_room_group_app_date_room_group_app_date_ro",
                        columns: x => new { x.room_group_id, x.app_date_id },
                        principalSchema: "public",
                        principalTable: "room_group_app_date",
                        principalColumns: new[] { "room_group_id", "app_date_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_plan_room_group_app_date_room_group_room_group_",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_plan_room_group_app_date_user_info_user_info_id",
                        column: x => x.user_info_id,
                        principalSchema: "public",
                        principalTable: "user_info",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", maxLength: 36, nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_info_id = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    memo = table.Column<string>(type: "text", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    record_memo = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_user_info_user_info_id",
                        column: x => x.user_info_id,
                        principalSchema: "public",
                        principalTable: "user_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservation_room_group_app_date_option_item",
                schema: "public",
                columns: table => new
                {
                    reservation_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_id = table.Column<long>(type: "bigint", nullable: false),
                    app_date_id = table.Column<long>(type: "bigint", nullable: false),
                    option_item_id = table.Column<long>(type: "bigint", nullable: false),
                    room_group_index = table.Column<int>(type: "integer", nullable: false),
                    rest_index = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<int>(type: "integer", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    reservation_plan_room_group_app_date_app_date_id = table.Column<long>(type: "bigint", nullable: true),
                    reservation_plan_room_group_app_date_plan_id = table.Column<long>(type: "bigint", nullable: true),
                    reservation_plan_room_group_app_date_reservation_id = table.Column<long>(type: "bigint", nullable: true),
                    reservation_plan_room_group_app_date_room_group_id = table.Column<long>(type: "bigint", nullable: true),
                    reservation_plan_room_group_app_date_room_group_index = table.Column<int>(type: "integer", nullable: true),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<long>(type: "bigint", nullable: true),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_at = table.Column<long>(type: "bigint", nullable: true),
                    record_memo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation_room_group_app_date_option_item", x => new { x.reservation_id, x.room_group_id, x.app_date_id, x.option_item_id, x.room_group_index });
                    table.ForeignKey(
                        name: "fk_reservation_room_group_app_date_option_item_app_date_app_da",
                        column: x => x.app_date_id,
                        principalSchema: "public",
                        principalTable: "app_date",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_room_group_app_date_option_item_option_item_app",
                        columns: x => new { x.option_item_id, x.app_date_id },
                        principalSchema: "public",
                        principalTable: "option_item_app_date",
                        principalColumns: new[] { "option_item_id", "app_date_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_room_group_app_date_option_item_option_item_opt",
                        column: x => x.option_item_id,
                        principalSchema: "public",
                        principalTable: "option_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_room_group_app_date_option_item_reservation_pla",
                        columns: x => new { x.reservation_plan_room_group_app_date_reservation_id, x.reservation_plan_room_group_app_date_plan_id, x.reservation_plan_room_group_app_date_room_group_id, x.reservation_plan_room_group_app_date_app_date_id, x.reservation_plan_room_group_app_date_room_group_index },
                        principalSchema: "public",
                        principalTable: "reservation_plan_room_group_app_date",
                        principalColumns: new[] { "reservation_id", "plan_id", "room_group_id", "app_date_id", "room_group_index" });
                    table.ForeignKey(
                        name: "fk_reservation_room_group_app_date_option_item_reservation_res",
                        column: x => x.reservation_id,
                        principalSchema: "public",
                        principalTable: "reservation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservation_room_group_app_date_option_item_room_group_room",
                        column: x => x.room_group_id,
                        principalSchema: "public",
                        principalTable: "room_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_allergen_code",
                schema: "public",
                table: "allergen",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_date_code",
                schema: "public",
                table: "app_date",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_date_app_date_data_app_date_data_id",
                schema: "public",
                table: "app_date_app_date_data",
                column: "app_date_data_id");

            migrationBuilder.CreateIndex(
                name: "ix_app_date_app_date_type_app_date_type_id",
                schema: "public",
                table: "app_date_app_date_type",
                column: "app_date_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_app_date_data_code",
                schema: "public",
                table: "app_date_data",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_date_type_code",
                schema: "public",
                table: "app_date_type",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_application_user_favorite_favorite_id",
                schema: "public",
                table: "application_user_favorite",
                column: "favorite_id");

            migrationBuilder.CreateIndex(
                name: "ix_application_user_login_history_login_history_id",
                schema: "public",
                table: "application_user_login_history",
                column: "login_history_id");

            migrationBuilder.CreateIndex(
                name: "ix_application_user_point_point_id",
                schema: "public",
                table: "application_user_point",
                column: "point_id");

            migrationBuilder.CreateIndex(
                name: "ix_area_code",
                schema: "public",
                table: "area",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_area_parent_id",
                schema: "public",
                table: "area",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_bed_type_code",
                schema: "public",
                table: "bed_type",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_calendar_code",
                schema: "public",
                table: "calendar",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_calendar_app_date_app_date_type_app_date_type_id",
                schema: "public",
                table: "calendar_app_date_app_date_type",
                column: "app_date_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_cancellation_code",
                schema: "public",
                table: "cancellation",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cancellation_cancellation_data_cancellation_data_id",
                schema: "public",
                table: "cancellation_cancellation_data",
                column: "cancellation_data_id");

            migrationBuilder.CreateIndex(
                name: "ix_cancellation_data_code",
                schema: "public",
                table: "cancellation_data",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_category_code",
                schema: "public",
                table: "category",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_category_parent_id",
                schema: "public",
                table: "category",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_consumption_tax_code",
                schema: "public",
                table: "consumption_tax",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_country_code",
                schema: "public",
                table: "country",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_discount_data_code",
                schema: "public",
                table: "discount_data",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_facility_area_id",
                schema: "public",
                table: "facility",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_category_id",
                schema: "public",
                table: "facility",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_code",
                schema: "public",
                table: "facility",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_facility_country_id",
                schema: "public",
                table: "facility",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_parent_id",
                schema: "public",
                table: "facility",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_allergen_allergen_id",
                schema: "public",
                table: "facility_allergen",
                column: "allergen_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_app_date_type_app_date_type_id",
                schema: "public",
                table: "facility_app_date_type",
                column: "app_date_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_application_user_user_id",
                schema: "public",
                table: "facility_application_user",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_calendar_calendar_id",
                schema: "public",
                table: "facility_calendar",
                column: "calendar_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_cancellation_cancellation_id",
                schema: "public",
                table: "facility_cancellation",
                column: "cancellation_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_category_category_id",
                schema: "public",
                table: "facility_category",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_fax_service_fax_service_id",
                schema: "public",
                table: "facility_fax_service",
                column: "fax_service_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_file_file_id",
                schema: "public",
                table: "facility_file",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_option_item_option_item_id",
                schema: "public",
                table: "facility_option_item",
                column: "option_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_person_age_type_person_age_type_id",
                schema: "public",
                table: "facility_person_age_type",
                column: "person_age_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_plan_plan_id",
                schema: "public",
                table: "facility_plan",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_question_question_id",
                schema: "public",
                table: "facility_question",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_room_group_room_group_id",
                schema: "public",
                table: "facility_room_group",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_site_site_id",
                schema: "public",
                table: "facility_site",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_facility_spa_tax_group_spa_tax_group_id",
                schema: "public",
                table: "facility_spa_tax_group",
                column: "spa_tax_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_favorite_code",
                schema: "public",
                table: "favorite",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_favorite_facility_id",
                schema: "public",
                table: "favorite",
                column: "facility_id");

            migrationBuilder.CreateIndex(
                name: "ix_favorite_plan_id",
                schema: "public",
                table: "favorite",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_fax_service_code",
                schema: "public",
                table: "fax_service",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_file_code",
                schema: "public",
                table: "file",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_file_category_category_id",
                schema: "public",
                table: "file_category",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_file_option_item_option_item_id",
                schema: "public",
                table: "file_option_item",
                column: "option_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_file_plan_plan_id",
                schema: "public",
                table: "file_plan",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_file_room_room_id",
                schema: "public",
                table: "file_room",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_file_room_group_room_group_id",
                schema: "public",
                table: "file_room_group",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_gmo_payment_result_request_code",
                schema: "public",
                table: "gmo_payment_result_request",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_login_history_code",
                schema: "public",
                table: "login_history",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_meal_type_code",
                schema: "public",
                table: "meal_type",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_option_item_code",
                schema: "public",
                table: "option_item",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_option_item_app_date_app_date_id",
                schema: "public",
                table: "option_item_app_date",
                column: "app_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_option_item_category_category_id",
                schema: "public",
                table: "option_item_category",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_option_item_question_question_id",
                schema: "public",
                table: "option_item_question",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_api_issue_code",
                schema: "public",
                table: "order",
                column: "api_issue_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_order_code",
                schema: "public",
                table: "order",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_order_gmo_payment_result_request_gmo_payment_result_request",
                schema: "public",
                table: "order_gmo_payment_result_request",
                column: "gmo_payment_result_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_gmo_payment_result_request_reservation_id",
                schema: "public",
                table: "order_gmo_payment_result_request",
                column: "reservation_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_reservation_reservation_id",
                schema: "public",
                table: "order_reservation",
                column: "reservation_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_age_type_code",
                schema: "public",
                table: "person_age_type",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_person_age_type_spa_tax_data_spa_tax_data_id",
                schema: "public",
                table: "person_age_type_spa_tax_data",
                column: "spa_tax_data_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_cancellation_id",
                schema: "public",
                table: "plan",
                column: "cancellation_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_code",
                schema: "public",
                table: "plan",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_plan_point_rate_id",
                schema: "public",
                table: "plan",
                column: "point_rate_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_category_category_id",
                schema: "public",
                table: "plan_category",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_meal_type_meal_type_id",
                schema: "public",
                table: "plan_meal_type",
                column: "meal_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_option_item_option_item_id",
                schema: "public",
                table: "plan_option_item",
                column: "option_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_question_question_id",
                schema: "public",
                table: "plan_question",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_room_group_id",
                schema: "public",
                table: "plan_room_group",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_cancellation_cancellation_id",
                schema: "public",
                table: "plan_room_group_cancellation",
                column: "cancellation_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_cancellation_room_group_id",
                schema: "public",
                table: "plan_room_group_cancellation",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_plan_room_group_plan_id_plan_room_grou",
                schema: "public",
                table: "plan_room_group_site",
                columns: new[] { "plan_room_group_plan_id", "plan_room_group_room_group_id" });

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_room_group_id",
                schema: "public",
                table: "plan_room_group_site",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_site_id",
                schema: "public",
                table: "plan_room_group_site",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_app_date_app_date_id",
                schema: "public",
                table: "plan_room_group_site_app_date",
                column: "app_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_app_date_room_group_id",
                schema: "public",
                table: "plan_room_group_site_app_date",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_app_date_site_id",
                schema: "public",
                table: "plan_room_group_site_app_date",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_app_date_price_data_app_date_id",
                schema: "public",
                table: "plan_room_group_site_app_date_price_data",
                column: "app_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_app_date_price_data_price_data_id",
                schema: "public",
                table: "plan_room_group_site_app_date_price_data",
                column: "price_data_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_app_date_price_data_room_group_id",
                schema: "public",
                table: "plan_room_group_site_app_date_price_data",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_app_date_price_data_site_id",
                schema: "public",
                table: "plan_room_group_site_app_date_price_data",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_app_date_type_price_data_app_date_type",
                schema: "public",
                table: "plan_room_group_site_app_date_type_price_data",
                column: "app_date_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_app_date_type_price_data_price_data_id",
                schema: "public",
                table: "plan_room_group_site_app_date_type_price_data",
                column: "price_data_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_app_date_type_price_data_room_group_id",
                schema: "public",
                table: "plan_room_group_site_app_date_type_price_data",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_app_date_type_price_data_site_id",
                schema: "public",
                table: "plan_room_group_site_app_date_type_price_data",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_discount_data_discount_data_id",
                schema: "public",
                table: "plan_room_group_site_discount_data",
                column: "discount_data_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_discount_data_room_group_id",
                schema: "public",
                table: "plan_room_group_site_discount_data",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_discount_data_site_id",
                schema: "public",
                table: "plan_room_group_site_discount_data",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_person_age_type_person_age_type_id",
                schema: "public",
                table: "plan_room_group_site_person_age_type",
                column: "person_age_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_person_age_type_room_group_id",
                schema: "public",
                table: "plan_room_group_site_person_age_type",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_room_group_site_person_age_type_site_id",
                schema: "public",
                table: "plan_room_group_site_person_age_type",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_site_site_id",
                schema: "public",
                table: "plan_site",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_point_code",
                schema: "public",
                table: "point",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_point_parent_id",
                schema: "public",
                table: "point",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_point_rate_code",
                schema: "public",
                table: "point_rate",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_prefecture_code",
                schema: "public",
                table: "prefecture",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_price_data_code",
                schema: "public",
                table: "price_data",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_que_code",
                schema: "public",
                table: "que",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_question_code",
                schema: "public",
                table: "question",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reservation_code",
                schema: "public",
                table: "reservation",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reservation_facility_id",
                schema: "public",
                table: "reservation",
                column: "facility_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_main_user_id",
                schema: "public",
                table: "reservation",
                column: "main_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_parent_id",
                schema: "public",
                table: "reservation",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_plan_id",
                schema: "public",
                table: "reservation",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_reserver_id",
                schema: "public",
                table: "reservation",
                column: "reserver_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_group_id",
                schema: "public",
                table: "reservation",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_site_id",
                schema: "public",
                table: "reservation",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_user_id",
                schema: "public",
                table: "reservation",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_plan_room_group_app_date_app_date_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                column: "app_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_plan_room_group_app_date_plan_id_room_group_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                columns: new[] { "plan_id", "room_group_id" });

            migrationBuilder.CreateIndex(
                name: "ix_reservation_plan_room_group_app_date_room_group_id_app_date",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                columns: new[] { "room_group_id", "app_date_id" });

            migrationBuilder.CreateIndex(
                name: "ix_reservation_plan_room_group_app_date_user_info_id",
                schema: "public",
                table: "reservation_plan_room_group_app_date",
                column: "user_info_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_point_point_id",
                schema: "public",
                table: "reservation_point",
                column: "point_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_question_question_id",
                schema: "public",
                table: "reservation_question",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_group_app_date_option_item_app_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                column: "app_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_group_app_date_option_item_option_item_id_",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                columns: new[] { "option_item_id", "app_date_id" });

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_group_app_date_option_item_reservation_pla",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                columns: new[] { "reservation_plan_room_group_app_date_reservation_id", "reservation_plan_room_group_app_date_plan_id", "reservation_plan_room_group_app_date_room_group_id", "reservation_plan_room_group_app_date_app_date_id", "reservation_plan_room_group_app_date_room_group_index" });

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_group_app_date_option_item_room_group_id",
                schema: "public",
                table: "reservation_room_group_app_date_option_item",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_group_app_date_person_age_type_app_date_id",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                column: "app_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_group_app_date_person_age_type_person_age_",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                column: "person_age_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_room_group_app_date_person_age_type_room_group_",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_code",
                schema: "public",
                table: "room",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_room_group_code",
                schema: "public",
                table: "room_group",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_room_group_app_date_app_date_id",
                schema: "public",
                table: "room_group_app_date",
                column: "app_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_app_date_type_price_data_app_date_type_id",
                schema: "public",
                table: "room_group_app_date_type_price_data",
                column: "app_date_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_app_date_type_price_data_price_data_id",
                schema: "public",
                table: "room_group_app_date_type_price_data",
                column: "price_data_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_bed_type_bed_type_id",
                schema: "public",
                table: "room_group_bed_type",
                column: "bed_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_category_category_id",
                schema: "public",
                table: "room_group_category",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_site_site_id",
                schema: "public",
                table: "room_group_site",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_site_app_date_app_date_id",
                schema: "public",
                table: "room_group_site_app_date",
                column: "app_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_site_app_date_site_id",
                schema: "public",
                table: "room_group_site_app_date",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_site_app_date_price_data_app_date_id",
                schema: "public",
                table: "room_group_site_app_date_price_data",
                column: "app_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_site_app_date_price_data_price_data_id",
                schema: "public",
                table: "room_group_site_app_date_price_data",
                column: "price_data_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_site_app_date_price_data_site_id",
                schema: "public",
                table: "room_group_site_app_date_price_data",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_site_app_date_type_price_data_app_date_type_id",
                schema: "public",
                table: "room_group_site_app_date_type_price_data",
                column: "app_date_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_site_app_date_type_price_data_price_data_id",
                schema: "public",
                table: "room_group_site_app_date_type_price_data",
                column: "price_data_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_group_site_app_date_type_price_data_site_id",
                schema: "public",
                table: "room_group_site_app_date_type_price_data",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_room_group_room_group_id",
                schema: "public",
                table: "room_room_group",
                column: "room_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_site_code",
                schema: "public",
                table: "site",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_site_point_rate_point_rate_id",
                schema: "public",
                table: "site_point_rate",
                column: "point_rate_id");

            migrationBuilder.CreateIndex(
                name: "ix_spa_tax_data_code",
                schema: "public",
                table: "spa_tax_data",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_spa_tax_group_code",
                schema: "public",
                table: "spa_tax_group",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_spa_tax_group_spa_tax_data_spa_tax_data_id",
                schema: "public",
                table: "spa_tax_group_spa_tax_data",
                column: "spa_tax_data_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_config_code",
                schema: "public",
                table: "system_config",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_code",
                schema: "public",
                table: "user",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_user_info_id",
                schema: "public",
                table: "user",
                column: "user_info_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_info_code",
                schema: "public",
                table: "user_info",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_info_country_id",
                schema: "public",
                table: "user_info",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_info_reservation_room_group_app_date_person_age_type_r",
                schema: "public",
                table: "user_info",
                columns: new[] { "reservation_room_group_app_date_person_age_type_reservation_id", "reservation_room_group_app_date_person_age_type_room_group_id", "reservation_room_group_app_date_person_age_type_app_date_id", "reservation_room_group_app_date_person_age_type_person_age_type_id", "reservation_room_group_app_date_person_age_type_room_group_index" });

            migrationBuilder.AddForeignKey(
                name: "fk_application_user_favorite_user_user_id",
                schema: "public",
                table: "application_user_favorite",
                column: "user_id",
                principalSchema: "public",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_application_user_login_history_user_user_id",
                schema: "public",
                table: "application_user_login_history",
                column: "user_id",
                principalSchema: "public",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_application_user_point_user_user_id",
                schema: "public",
                table: "application_user_point",
                column: "user_id",
                principalSchema: "public",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_facility_application_user_user_user_id",
                schema: "public",
                table: "facility_application_user",
                column: "user_id",
                principalSchema: "public",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_order_gmo_payment_result_request_reservation_reservation_id",
                schema: "public",
                table: "order_gmo_payment_result_request",
                column: "reservation_id",
                principalSchema: "public",
                principalTable: "reservation",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_reservation_reservation_reservation_id",
                schema: "public",
                table: "order_reservation",
                column: "reservation_id",
                principalSchema: "public",
                principalTable: "reservation",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_user_info_main_user_id",
                schema: "public",
                table: "reservation",
                column: "main_user_id",
                principalSchema: "public",
                principalTable: "user_info",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_user_info_reserver_id",
                schema: "public",
                table: "reservation",
                column: "reserver_id",
                principalSchema: "public",
                principalTable: "user_info",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_user_user_id",
                schema: "public",
                table: "reservation",
                column: "user_id",
                principalSchema: "public",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_app_date_ap",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_user_user_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_plan_cancellation_cancellation_id",
                schema: "public",
                table: "plan");

            migrationBuilder.DropForeignKey(
                name: "fk_facility_area_area_id",
                schema: "public",
                table: "facility");

            migrationBuilder.DropForeignKey(
                name: "fk_facility_category_category_id",
                schema: "public",
                table: "facility");

            migrationBuilder.DropForeignKey(
                name: "fk_facility_country_country_id",
                schema: "public",
                table: "facility");

            migrationBuilder.DropForeignKey(
                name: "fk_user_info_country_country_id",
                schema: "public",
                table: "user_info");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_facility_facility_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_person_age_",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_plan_plan_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_room_group_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_room_group_",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_site_site_id",
                schema: "public",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_room_group_app_date_person_age_type_reservation",
                schema: "public",
                table: "reservation_room_group_app_date_person_age_type");

            migrationBuilder.DropTable(
                name: "app_date_app_date_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "app_date_app_date_type",
                schema: "public");

            migrationBuilder.DropTable(
                name: "application_user_favorite",
                schema: "public");

            migrationBuilder.DropTable(
                name: "application_user_login_history",
                schema: "public");

            migrationBuilder.DropTable(
                name: "application_user_point",
                schema: "public");

            migrationBuilder.DropTable(
                name: "calendar_app_date_app_date_type",
                schema: "public");

            migrationBuilder.DropTable(
                name: "cancellation_cancellation_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "consumption_tax",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_allergen",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_app_date_type",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_application_user",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_calendar",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_cancellation",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_category",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_fax_service",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_file",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_option_item",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_person_age_type",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_plan",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_question",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_room_group",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_site",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility_spa_tax_group",
                schema: "public");

            migrationBuilder.DropTable(
                name: "file_category",
                schema: "public");

            migrationBuilder.DropTable(
                name: "file_option_item",
                schema: "public");

            migrationBuilder.DropTable(
                name: "file_plan",
                schema: "public");

            migrationBuilder.DropTable(
                name: "file_room",
                schema: "public");

            migrationBuilder.DropTable(
                name: "file_room_group",
                schema: "public");

            migrationBuilder.DropTable(
                name: "option_item_category",
                schema: "public");

            migrationBuilder.DropTable(
                name: "option_item_question",
                schema: "public");

            migrationBuilder.DropTable(
                name: "order_gmo_payment_result_request",
                schema: "public");

            migrationBuilder.DropTable(
                name: "order_reservation",
                schema: "public");

            migrationBuilder.DropTable(
                name: "person_age_type_spa_tax_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan_category",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan_meal_type",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan_option_item",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan_question",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan_room_group_cancellation",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan_room_group_site",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan_room_group_site_app_date",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan_room_group_site_app_date_price_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan_room_group_site_app_date_type_price_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan_room_group_site_discount_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan_room_group_site_person_age_type",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan_site",
                schema: "public");

            migrationBuilder.DropTable(
                name: "prefecture",
                schema: "public");

            migrationBuilder.DropTable(
                name: "que",
                schema: "public");

            migrationBuilder.DropTable(
                name: "reservation_point",
                schema: "public");

            migrationBuilder.DropTable(
                name: "reservation_question",
                schema: "public");

            migrationBuilder.DropTable(
                name: "reservation_room_group_app_date_option_item",
                schema: "public");

            migrationBuilder.DropTable(
                name: "room_group_app_date_type_price_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "room_group_bed_type",
                schema: "public");

            migrationBuilder.DropTable(
                name: "room_group_category",
                schema: "public");

            migrationBuilder.DropTable(
                name: "room_group_site",
                schema: "public");

            migrationBuilder.DropTable(
                name: "room_group_site_app_date",
                schema: "public");

            migrationBuilder.DropTable(
                name: "room_group_site_app_date_price_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "room_group_site_app_date_type_price_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "room_room_group",
                schema: "public");

            migrationBuilder.DropTable(
                name: "site_point_rate",
                schema: "public");

            migrationBuilder.DropTable(
                name: "spa_tax_group_spa_tax_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "system_config",
                schema: "public");

            migrationBuilder.DropTable(
                name: "app_date_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "favorite",
                schema: "public");

            migrationBuilder.DropTable(
                name: "login_history",
                schema: "public");

            migrationBuilder.DropTable(
                name: "cancellation_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "allergen",
                schema: "public");

            migrationBuilder.DropTable(
                name: "calendar",
                schema: "public");

            migrationBuilder.DropTable(
                name: "fax_service",
                schema: "public");

            migrationBuilder.DropTable(
                name: "file",
                schema: "public");

            migrationBuilder.DropTable(
                name: "gmo_payment_result_request",
                schema: "public");

            migrationBuilder.DropTable(
                name: "order",
                schema: "public");

            migrationBuilder.DropTable(
                name: "meal_type",
                schema: "public");

            migrationBuilder.DropTable(
                name: "discount_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "point",
                schema: "public");

            migrationBuilder.DropTable(
                name: "question",
                schema: "public");

            migrationBuilder.DropTable(
                name: "option_item_app_date",
                schema: "public");

            migrationBuilder.DropTable(
                name: "reservation_plan_room_group_app_date",
                schema: "public");

            migrationBuilder.DropTable(
                name: "bed_type",
                schema: "public");

            migrationBuilder.DropTable(
                name: "app_date_type",
                schema: "public");

            migrationBuilder.DropTable(
                name: "price_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "room",
                schema: "public");

            migrationBuilder.DropTable(
                name: "spa_tax_data",
                schema: "public");

            migrationBuilder.DropTable(
                name: "spa_tax_group",
                schema: "public");

            migrationBuilder.DropTable(
                name: "option_item",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan_room_group",
                schema: "public");

            migrationBuilder.DropTable(
                name: "room_group_app_date",
                schema: "public");

            migrationBuilder.DropTable(
                name: "app_date",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user",
                schema: "public");

            migrationBuilder.DropTable(
                name: "cancellation",
                schema: "public");

            migrationBuilder.DropTable(
                name: "area",
                schema: "public");

            migrationBuilder.DropTable(
                name: "category",
                schema: "public");

            migrationBuilder.DropTable(
                name: "country",
                schema: "public");

            migrationBuilder.DropTable(
                name: "facility",
                schema: "public");

            migrationBuilder.DropTable(
                name: "person_age_type",
                schema: "public");

            migrationBuilder.DropTable(
                name: "plan",
                schema: "public");

            migrationBuilder.DropTable(
                name: "point_rate",
                schema: "public");

            migrationBuilder.DropTable(
                name: "room_group",
                schema: "public");

            migrationBuilder.DropTable(
                name: "site",
                schema: "public");

            migrationBuilder.DropTable(
                name: "reservation",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_info",
                schema: "public");

            migrationBuilder.DropTable(
                name: "reservation_room_group_app_date_person_age_type",
                schema: "public");
        }
    }
}
