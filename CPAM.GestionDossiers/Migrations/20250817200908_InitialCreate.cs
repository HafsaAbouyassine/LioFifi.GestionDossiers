using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CPAM.GestionDossiers.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dossiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomAssure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroIdentite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateNaissance = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Statut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categorie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgentResponsable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateDerniereModification = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontantRembourse = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PourcentageRemboursement = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Commentaires = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dossiers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Dossiers",
                columns: new[] { "Id", "AgentResponsable", "Categorie", "Commentaires", "DateCreation", "DateDerniereModification", "DateNaissance", "MontantRembourse", "NomAssure", "NumeroIdentite", "PourcentageRemboursement", "Statut" },
                values: new object[,]
                {
                    { 1, "Alice Martin", "Santé", "Dossier en cours de traitement.", new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1985, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 150.00m, "Jean Dupont", "D123456", 75.00m, "En cours" },
                    { 2, "Bob Lefevre", "Retraite", "Dossier terminé avec succès.", new DateTime(2023, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1970, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 200.00m, "Marie Curie", "D654321", 100.00m, "Terminé" },
                    { 3, "Claire Dubois", "Invalidité", "En attente de documents supplémentaires.", new DateTime(2023, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.00m, "Pierre Martin", "D789012", 0.00m, "En attente" },
                    { 4, "David Moreau", "Santé", "Dossier en cours de traitement, attente de pièces justificatives.", new DateTime(2023, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1988, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 300.00m, "Sophie Durand", "D345678", 60.00m, "En cours" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dossiers");
        }
    }
}
