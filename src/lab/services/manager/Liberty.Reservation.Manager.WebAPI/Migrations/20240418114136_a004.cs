using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.Employee.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class a004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "FileEmployees",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "関係種類");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "FileEmployees");
        }
    }
}
