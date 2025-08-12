using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CPAM.GestionDossiers.Migrations
{
    /// <inheritdoc />
    public partial class SeedDossiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Dossiers",
                columns: new[] { "Id", "DateCreation", "NomAssure", "NumeroIdentite", "Statut" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jean Dupont", "D123456", "En cours" },
                    { 2, new DateTime(2023, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marie Curie", "D654321", "Terminé" },
                    { 3, new DateTime(2023, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pierre Martin", "D789012", "En attente" },
                    { 4, new DateTime(2023, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sophie Durand", "D345678", "En cours" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Dossiers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Dossiers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Dossiers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Dossiers",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
