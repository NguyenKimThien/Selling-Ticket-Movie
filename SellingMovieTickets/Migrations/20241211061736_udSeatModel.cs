using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellingMovieTickets.Migrations
{
    public partial class udSeatModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerManagements_AspNetUsers_AppUserId",
                table: "CustomerManagements");

            migrationBuilder.DropIndex(
                name: "IX_CustomerManagements_AppUserId",
                table: "CustomerManagements");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OtherServices");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "CustomerManagements");

            migrationBuilder.AddColumn<int>(
                name: "HeldByUserId",
                table: "Seats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoldUntil",
                table: "Seats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsHeld",
                table: "Seats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "OtherServices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CustomerManagements",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerManagements_UserId",
                table: "CustomerManagements",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerManagements_AspNetUsers_UserId",
                table: "CustomerManagements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerManagements_AspNetUsers_UserId",
                table: "CustomerManagements");

            migrationBuilder.DropIndex(
                name: "IX_CustomerManagements_UserId",
                table: "CustomerManagements");

            migrationBuilder.DropColumn(
                name: "HeldByUserId",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "HoldUntil",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "IsHeld",
                table: "Seats");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "OtherServices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OtherServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CustomerManagements",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "CustomerManagements",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerManagements_AppUserId",
                table: "CustomerManagements",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerManagements_AspNetUsers_AppUserId",
                table: "CustomerManagements",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
