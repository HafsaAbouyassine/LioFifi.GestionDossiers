using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CPAM.GestionDossiers.Migrations
{
    /// <inheritdoc />
    public partial class AjoutColonnesDossier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgentResponsable",
                table: "Dossiers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Categorie",
                table: "Dossiers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDerniereModification",
                table: "Dossiers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Dossiers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AgentResponsable", "Categorie", "DateDerniereModification" },
                values: new object[] { "Alice Martin", "Santé", new DateTime(2023, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Dossiers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AgentResponsable", "Categorie", "DateDerniereModification" },
                values: new object[] { "Bob Lefevre", "Retraite", new DateTime(2023, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Dossiers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AgentResponsable", "Categorie", "DateDerniereModification" },
                values: new object[] { "Claire Dubois", "Invalidité", new DateTime(2023, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Dossiers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AgentResponsable", "Categorie", "DateDerniereModification" },
                values: new object[] { "David Moreau", "Santé", new DateTime(2023, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentResponsable",
                table: "Dossiers");

            migrationBuilder.DropColumn(
                name: "Categorie",
                table: "Dossiers");

            migrationBuilder.DropColumn(
                name: "DateDerniereModification",
                table: "Dossiers");
        }
    }
}
