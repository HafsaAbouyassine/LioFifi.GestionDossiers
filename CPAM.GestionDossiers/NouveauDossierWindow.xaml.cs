using CPAM.GestionDossiers.Data;
using CPAM.GestionDossiers.Models;
using System;
using System.Windows;

namespace CPAM.GestionDossiers
{
    public partial class NouveauDossierWindow : Window
    {
        private readonly AppDbContext _context;

        // Enlevez le paramètre du constructeur
        public NouveauDossierWindow()
        {
            InitializeComponent();
            _context = new AppDbContext(); // Créez le contexte ici
        }

        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Votre logique de sauvegarde ici
            try
            {
                // Exemple de création d'un nouveau dossier
                var nouveauDossier = new Dossier
                {
                    NumeroIdentite = "NouveauNumero",
                    NomAssure = "NouveauNom",
                    DateCreation = DateTime.Now,
                    Statut = "Nouveau",
                    DateDerniereModification = DateTime.Now
                    // Ajoutez les autres propriétés
                };

                _context.Dossiers.Add(nouveauDossier);
                _context.SaveChanges();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}