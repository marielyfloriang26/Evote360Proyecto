using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evote360.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDescripcionAPuestoElectivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "PuestosElectivos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "PuestosElectivos");
        }
    }
}
