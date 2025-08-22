
using CPAM.GestionDossiers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace CPAM.GestionDossiers.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Dossier> Dossiers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 👇 constructeur vide pour le designer
        public AppDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CPAMDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Dossier>().HasData(
                new Dossier
                {
                    Id = 1,
                    NomAssure = "Jean Dupont",
                    NumeroIdentite = "D123456",
                    DateCreation = new DateTime(2023, 1, 15),
                    DateNaissance = new DateTime(1985, 6, 20),
                    DateDerniereModification = new DateTime(2023, 1, 20),
                    Statut = "En cours",
                    Categorie = "Santé",
                    AgentResponsable = "Alice Martin",
                    MontantRembourse = 150.00m,
                    PourcentageRemboursement = 75.00m,
                    Commentaires = "Dossier en cours de traitement."
                },
                new Dossier
                {
                    Id = 2,
                    NomAssure = "Marie Curie",
                    NumeroIdentite = "D654321",
                    DateCreation = new DateTime(2023, 2, 20),
                    DateNaissance = new DateTime(1970, 11, 7),
                    DateDerniereModification = new DateTime(2023, 2, 25),
                    Statut = "Terminé",
                    Categorie = "Retraite",
                    AgentResponsable = "Bob Lefevre",
                    MontantRembourse = 200.00m,
                    PourcentageRemboursement = 100.00m,
                    Commentaires = "Dossier terminé avec succès."

                },
                new Dossier
                {
                    Id = 3,
                    NomAssure = "Pierre Martin",
                    NumeroIdentite = "D789012",
                    DateCreation = new DateTime(2023, 3, 10),
                    DateNaissance = new DateTime(1990, 3, 15),
                    DateDerniereModification = new DateTime(2023, 3, 15),
                    Statut = "En attente",
                    Categorie = "Invalidité",
                    AgentResponsable = "Claire Dubois",
                    MontantRembourse = 0.00m,
                    PourcentageRemboursement = 0.00m,
                    Commentaires = "En attente de documents supplémentaires."
                },
                new Dossier
                {
                    Id = 4,
                    NomAssure = "Sophie Durand",
                    NumeroIdentite = "D345678",
                    DateCreation = new DateTime(2023, 4, 5),
                    DateNaissance = new DateTime(1988, 8, 30),
                    DateDerniereModification = new DateTime(2023, 4, 10),
                    Statut = "En cours",
                    Categorie = "Santé",
                    AgentResponsable = "David Moreau",
                    MontantRembourse = 300.00m,
                    PourcentageRemboursement = 60.00m,
                    Commentaires = "Dossier en cours de traitement, attente de pièces justificatives."
                }
            );
        }
    }
}
