using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee_Attendance_Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dolgozok",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nev = table.Column<string>(type: "TEXT", nullable: false),
                    FelhasznaloNev = table.Column<string>(type: "TEXT", nullable: false),
                    JelszoHash = table.Column<string>(type: "TEXT", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dolgozok", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HaviMunka",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DolgozoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Datum = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LedolgozottIdoPerc = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HaviMunka", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HaviMunka_Dolgozok_DolgozoId",
                        column: x => x.DolgozoId,
                        principalTable: "Dolgozok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Munkaorak",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DolgozoId = table.Column<int>(type: "INTEGER", nullable: false),
                    BelepesIdo = table.Column<DateTime>(type: "TEXT", nullable: false),
                    KilepesIdo = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Munkaorak", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Munkaorak_Dolgozok_DolgozoId",
                        column: x => x.DolgozoId,
                        principalTable: "Dolgozok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HaviMunka_DolgozoId",
                table: "HaviMunka",
                column: "DolgozoId");

            migrationBuilder.CreateIndex(
                name: "IX_Munkaorak_DolgozoId",
                table: "Munkaorak",
                column: "DolgozoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HaviMunka");

            migrationBuilder.DropTable(
                name: "Munkaorak");

            migrationBuilder.DropTable(
                name: "Dolgozok");
        }
    }
}
