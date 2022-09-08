using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeSimulatorBackend.Migrations
{
    public partial class addAccountBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AccountBalance",
                schema: "public",
                table: "AspNetUsers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountBalance",
                schema: "public",
                table: "AspNetUsers");
        }
    }
}
