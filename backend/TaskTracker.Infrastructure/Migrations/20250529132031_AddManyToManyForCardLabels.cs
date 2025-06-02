using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddManyToManyForCardLabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Cards_CardId",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Labels_CardId",
                table: "Labels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardLabels",
                table: "CardLabels");

            migrationBuilder.DropIndex(
                name: "IX_CardLabels_CardId",
                table: "CardLabels");

            migrationBuilder.DropColumn(
                name: "CardId",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CardLabels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardLabels",
                table: "CardLabels",
                columns: new[] { "CardId", "LabelId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CardLabels",
                table: "CardLabels");

            migrationBuilder.AddColumn<Guid>(
                name: "CardId",
                table: "Labels",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CardLabels",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardLabels",
                table: "CardLabels",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CardId",
                table: "Labels",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardLabels_CardId",
                table: "CardLabels",
                column: "CardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Cards_CardId",
                table: "Labels",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
