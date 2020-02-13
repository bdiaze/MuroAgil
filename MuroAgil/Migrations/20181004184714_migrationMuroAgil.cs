using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MuroAgil.Migrations
{
    public partial class migrationMuroAgil : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MURO",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FECHA_CREACION = table.Column<DateTime>(type: "datetime", nullable: false),
                    FECHA_ULTIMA_MODIFICACION = table.Column<DateTime>(type: "datetime", nullable: false),
                    NOMBRE = table.Column<string>(unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MURO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CODIGO = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    HASH_CONTRASENNA = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    NOMBRE = table.Column<string>(unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ETAPA",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ID_MURO = table.Column<int>(nullable: false),
                    NOMBRE = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    POSICION = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETAPA", x => x.ID);
                    table.ForeignKey(
                        name: "FK__ETAPA__ID_MURO__22401542",
                        column: x => x.ID_MURO,
                        principalTable: "MURO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO_MURO",
                columns: table => new
                {
                    ID_DUENNO = table.Column<int>(nullable: false),
                    ID_MURO = table.Column<int>(nullable: false),
                    PERMISO = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO_MURO", x => new { x.ID_DUENNO, x.ID_MURO });
                    table.ForeignKey(
                        name: "FK__USUARIO_M__ID_DU__1E6F845E",
                        column: x => x.ID_DUENNO,
                        principalTable: "USUARIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__USUARIO_M__ID_MU__1F63A897",
                        column: x => x.ID_MURO,
                        principalTable: "MURO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TAREA",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DESCRIPCION = table.Column<string>(unicode: false, nullable: false),
                    FAMILIA = table.Column<short>(nullable: false),
                    ID_ETAPA = table.Column<int>(nullable: false),
                    POSICION = table.Column<short>(nullable: false),
                    TITULO = table.Column<string>(unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAREA", x => x.ID);
                    table.ForeignKey(
                        name: "FK__TAREA__ID_ETAPA__251C81ED",
                        column: x => x.ID_ETAPA,
                        principalTable: "ETAPA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ETAPA_ID_MURO",
                table: "ETAPA",
                column: "ID_MURO");

            migrationBuilder.CreateIndex(
                name: "IX_TAREA_ID_ETAPA",
                table: "TAREA",
                column: "ID_ETAPA");

            migrationBuilder.CreateIndex(
                name: "UQ__USUARIO__CC87E1260BB751F7",
                table: "USUARIO",
                column: "CODIGO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_MURO_ID_MURO",
                table: "USUARIO_MURO",
                column: "ID_MURO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TAREA");

            migrationBuilder.DropTable(
                name: "USUARIO_MURO");

            migrationBuilder.DropTable(
                name: "ETAPA");

            migrationBuilder.DropTable(
                name: "USUARIO");

            migrationBuilder.DropTable(
                name: "MURO");
        }
    }
}
