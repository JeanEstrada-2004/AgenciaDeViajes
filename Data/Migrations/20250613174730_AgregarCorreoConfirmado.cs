using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgenciaDeViajes.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCorreoConfirmado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CorreoConfirmado",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorreoConfirmado",
                table: "Usuarios");
        }
    }
}
