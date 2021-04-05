using Microsoft.EntityFrameworkCore.Migrations;

namespace checkpanel.Migrations
{
    public partial class DaysOfTheWeek : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Friday",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Monday",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Saturday",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Sunday",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Thursday",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Tuesday",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Wednesday",
                table: "Items",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Friday",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Monday",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Saturday",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Sunday",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Thursday",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Tuesday",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Wednesday",
                table: "Items");
        }
    }
}
