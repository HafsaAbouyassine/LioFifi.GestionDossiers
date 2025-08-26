using CPAM.GestionDossiers.Data;
using CPAM.GestionDossiers.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CPAM.GestionDossiers
{
    public partial class RechercherDossierWindow : Window
    {
        private readonly AppDbContext _context;
        public Dossier DossierSelectionne { get; private set; }

        public RechercherDossierWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            txtRecherche.Focus();
        }

        private void BtnRechercher_Click(object sender, RoutedEventArgs e)
        {
            EffectuerRecherche();
        }

        private void TxtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnOuvrir.IsEnabled = false;
        }

        private void LstResultats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnOuvrir.IsEnabled = lstResultats.SelectedItem != null;
        }

        private void BtnOuvrir_Click(object sender, RoutedEventArgs e)
        {
            if (lstResultats.SelectedItem is Dossier dossier)
            {
                DossierSelectionne = dossier;
                DialogResult = true;

                // Fermeture de cette fenêtre d'abord
                Close();
                var detailsWindow = new DossierDetailsPage(dossier);
                detailsWindow.ShowDialog();

            }
        }

        private void BtnFermer_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void EffectuerRecherche()
        {
            string recherche = txtRecherche.Text.Trim();

            if (string.IsNullOrEmpty(recherche))
            {
                MessageBox.Show("Veuillez entrer un critère de recherche (nom ou numéro).",
                    "Recherche vide", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var resultats = _context.Dossiers
                    .Where(d => d.NumeroIdentite.Contains(recherche) ||
                               d.NomAssure.Contains(recherche))
                    .OrderByDescending(d => d.DateCreation)
                    .ToList();

                lstResultats.ItemsSource = resultats;

                if (!resultats.Any())
                {
                    MessageBox.Show("Aucun dossier ne correspond à votre recherche.",
                        "Aucun résultat", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur est survenue lors de la recherche: {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
