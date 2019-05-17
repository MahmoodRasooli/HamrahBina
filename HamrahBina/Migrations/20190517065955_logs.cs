using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HamrahBina.Migrations
{
    public partial class logs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.CreateTable(
                name: "ApiCallLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Input = table.Column<string>(nullable: false),
                    Output = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiCallLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OcrCallLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Input = table.Column<string>(nullable: true),
                    Output = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    OcrName = table.Column<string>(nullable: true),
                    ResponseDate = table.Column<DateTime>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    ApiCallLogId = table.Column<Guid>(nullable: false),
                    IsSuccessful = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcrCallLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiCallLogs");

            migrationBuilder.DropTable(
                name: "OcrCallLogs");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "AspNetUserRoles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId",
                table: "AspNetUserRoles",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId",
                table: "AspNetUserRoles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
