using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TorneoFutbol.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTarjetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tarjetas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Minuto = table.Column<int>(type: "int", nullable: false),
                    PartidoId = table.Column<int>(type: "int", nullable: false),
                    JugadorId = table.Column<int>(type: "int", nullable: false),
                    EquipoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarjetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tarjetas_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tarjetas_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tarjetas_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tarjetas_EquipoId",
                table: "Tarjetas",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarjetas_JugadorId",
                table: "Tarjetas",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarjetas_PartidoId",
                table: "Tarjetas",
                column: "PartidoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tarjetas");
        }
    }
}
