using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MuroAgil.Migrations
{
    public partial class NombreCorreo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CODIGO",
                table: "USUARIO",
                newName: "CORREO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CORREO",
                table: "USUARIO",
                newName: "CODIGO");
        }
    }
}
