using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLabelsToCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CardId",
                table: "Labels",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CardId",
                table: "Labels",
                column: "CardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Cards_CardId",
                table: "Labels",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Cards_CardId",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Labels_CardId",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "CardId",
                table: "Labels");
        }
    }
}
