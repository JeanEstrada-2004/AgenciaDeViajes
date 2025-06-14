using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AgenciaDeViajes.Data.Migrations
{
    public partial class CrearTablaReviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    IdReview = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdDestino = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CalificacionAtencion = table.Column<int>(type: "integer", nullable: false),
                    CalificacionCalidad = table.Column<int>(type: "integer", nullable: false),
                    CalificacionPuntualidad = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Sentimiento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.IdReview);
                    table.ForeignKey(
                        name: "FK_Reviews_Destinos_IdDestino",
                        column: x => x.IdDestino,
                        principalTable: "Destinos",
                        principalColumn: "id_destino",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_IdDestino",
                table: "Reviews",
                column: "IdDestino");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");
        }
    }
}
