using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdateByForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
        name: "FK_Categories_Users_UpdateBy",
        table: "Categories",
        column: "UpdateBy",
        principalTable: "Users",
        principalColumn: "Id",
        onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_UpdateBy",
                table: "Transactions",
                column: "UpdateBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringExpenses_Users_UpdateBy",
                table: "RecurringExpenses",
                column: "UpdateBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_Users_UpdateBy",
                table: "Wallet",
                column: "UpdateBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "Users",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
        name: "FK_Categories_Users_UpdateBy",
        table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_UpdateBy",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_RecurringExpenses_Users_UpdateBy",
                table: "RecurringExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_Users_UpdateBy",
                table: "Wallet");
            migrationBuilder.AlterColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "Users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);
        }
    }
}
