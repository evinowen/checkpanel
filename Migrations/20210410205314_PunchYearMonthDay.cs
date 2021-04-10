using Microsoft.EntityFrameworkCore.Migrations;

namespace checkpanel.Migrations
{
    public partial class PunchYearMonthDay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "Punches",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Punches",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Punches",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "Punches");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Punches");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Punches");
        }
    }
}
