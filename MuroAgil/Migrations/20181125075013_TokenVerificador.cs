using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MuroAgil.Migrations
{
    public partial class TokenVerificador : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HASH_VALIDACION",
                table: "USUARIO",
                newName: "TOKEN_VERIFICADOR");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TOKEN_VERIFICADOR",
                table: "USUARIO",
                newName: "HASH_VALIDACION");
        }
    }
}
