using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellingMovieTickets.Migrations
{
    public partial class udSeat5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Seats");

            migrationBuilder.AddColumn<string>(
                name: "HeldByUserId",
                table: "Seats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeldByUserId",
                table: "Seats");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Seats",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
