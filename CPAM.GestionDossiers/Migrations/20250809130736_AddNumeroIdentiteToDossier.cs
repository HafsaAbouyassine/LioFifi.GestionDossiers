using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPAM.GestionDossiers.Migrations
{
    /// <inheritdoc />
    public partial class AddNumeroIdentiteToDossier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
               name: "NumeroIdentite",
               table: "Dossiers",
               type: "nvarchar(max)",
               nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroIdentite",
                table: "Dossiers");
        }
    }
}
