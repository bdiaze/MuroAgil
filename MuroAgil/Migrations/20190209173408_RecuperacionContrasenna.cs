using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MuroAgil.Migrations
{
    public partial class RecuperacionContrasenna : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FECHA_RECUP_CONTR",
                table: "USUARIO",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TOKEN_RECUP_CONTR",
                table: "USUARIO",
                unicode: false,
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FECHA_RECUP_CONTR",
                table: "USUARIO");

            migrationBuilder.DropColumn(
                name: "TOKEN_RECUP_CONTR",
                table: "USUARIO");
        }
    }
}
