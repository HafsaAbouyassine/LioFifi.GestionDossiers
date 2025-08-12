using CPAM.GestionDossiers.Data;
using CPAM.GestionDossiers.Models;
using System;
using System.Windows;

namespace CPAM.GestionDossiers
{
    public partial class DossierDetailsPage : Window
    {
        private readonly AppDbContext _context;
        private readonly Dossier _originalDossier;
        public Dossier CurrentDossier { get; set; }

        public DossierDetailsPage(Dossier dossier)
        {
            InitializeComponent();

            // Créer une copie du dossier pour l'édition
            _originalDossier = dossier;
            CurrentDossier = new Dossier
            {
                Id = dossier.Id,
                NomAssure = dossier.NomAssure,
                NumeroIdentite = dossier.NumeroIdentite,
                DateCreation = dossier.DateCreation,
                DateNaissance = dossier.DateNaissance,
                Statut = dossier.Statut,
                Categorie = dossier.Categorie,
                AgentResponsable = dossier.AgentResponsable,
                DateDerniereModification = dossier.DateDerniereModification,
                MontantRembourse = dossier.MontantRembourse,
                PourcentageRemboursement = dossier.PourcentageRemboursement,
                Commentaires = dossier.Commentaires,
            };

            DataContext = CurrentDossier;
            _context = new AppDbContext(); // Ou injection de dépendance
        }

        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(CurrentDossier.Statut))
                {
                    MessageBox.Show("Le statut est obligatoire.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Mettre à jour la date de modification
                CurrentDossier.DateDerniereModification = DateTime.Now;

                // Copier les modifications vers l'original
                _originalDossier.Statut = CurrentDossier.Statut;
                _originalDossier.Categorie = CurrentDossier.Categorie;
                _originalDossier.AgentResponsable = CurrentDossier.AgentResponsable;
                _originalDossier.MontantRembourse = CurrentDossier.MontantRembourse;
                _originalDossier.PourcentageRemboursement = CurrentDossier.PourcentageRemboursement;
                _originalDossier.Commentaires = CurrentDossier.Commentaires;
                _originalDossier.DateDerniereModification = CurrentDossier.DateDerniereModification;

                // Sauvegarder
                _context.Dossiers.Update(_originalDossier);
                _context.SaveChanges();

                MessageBox.Show("Modifications enregistrées avec succès.",
                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement: {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Annuler les modifications ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _context?.Dispose();
        }
    }
}