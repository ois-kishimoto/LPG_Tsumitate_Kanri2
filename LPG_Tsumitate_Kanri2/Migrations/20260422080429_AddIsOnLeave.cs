using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LPG_Tsumitate_Kanri2.Migrations
{
    /// <inheritdoc />
    public partial class AddIsOnLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnLeave",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnLeave",
                table: "Employees");
        }
    }
}
