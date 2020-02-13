using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MuroAgil.Migrations
{
    public partial class TareaColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "BLUE",
                table: "TAREA",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "GREEN",
                table: "TAREA",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "RED",
                table: "TAREA",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BLUE",
                table: "TAREA");

            migrationBuilder.DropColumn(
                name: "GREEN",
                table: "TAREA");

            migrationBuilder.DropColumn(
                name: "RED",
                table: "TAREA");
        }
    }
}
