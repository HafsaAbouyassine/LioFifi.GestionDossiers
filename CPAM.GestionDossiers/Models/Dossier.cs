
using System.ComponentModel.DataAnnotations;

namespace CPAM.GestionDossiers.Models
{
    public class Dossier
    {
        [Key]
        public int Id { get; set; }
        public string NomAssure { get; set; }
        public string NumeroIdentite { get; set; }
        public DateTime DateCreation { get; set; }

        public DateTime? DateNaissance { get; set; } 
        public string Statut { get; set; }

        public string Categorie { get; set; }
        public string AgentResponsable { get; set; }

        public DateTime DateDerniereModification { get; set; }

        public decimal MontantRembourse { get; set; }
        public decimal PourcentageRemboursement { get; set; }
        public string Commentaires { get; set; }




    }
}
