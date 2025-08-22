using CPAM.GestionDossiers.Data;
using CPAM.GestionDossiers.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace CPAM.GestionDossiers
{
    public partial class PageDossiers : Page
    {
        private readonly AppDbContext _context;
        private IQueryable<Dossier> _dossiersQuery;
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public PageDossiers()
        {
            InitializeComponent();
            _context = new AppDbContext();
            ChargerDossiers();
        }

        private void ChargerDossiers()
        {
            try
            {
                _dossiersQuery = _context.Dossiers
                    .OrderByDescending(d => d.DateCreation)
                    .AsQueryable();

                AppliquerFiltres();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des dossiers: {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AppliquerFiltres()
        {
            var query = _dossiersQuery;

            // Filtre par recherche texte
            if (!string.IsNullOrEmpty(txtRecherche.Text) && txtRecherche.Text != (string)txtRecherche.Tag)
            {
                var recherche = txtRecherche.Text.ToLower();
                query = query.Where(d =>
                    d.NomAssure.ToLower().Contains(recherche) ||
                    d.NumeroIdentite.ToLower().Contains(recherche));
            }

            // Filtre par statut
            if (cmbFiltreStatut.SelectedItem is ComboBoxItem selectedItem &&
                selectedItem.Content.ToString() != "Tous les statuts")
            {
                var statut = selectedItem.Content.ToString();
                query = query.Where(d => d.Statut == statut);
            }

            var resultats = query.ToList();
            dgDossiers.ItemsSource = resultats;
            txtCount.Text = $"{resultats.Count} dossiers";
        }

        // Ces méthodes doivent EXISTER pour correspondre aux événements XAML
        private void TxtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            AppliquerFiltres();
        }

        private void CmbFiltreStatut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFiltreStatut.IsLoaded)
            {
                AppliquerFiltres();
            }
        }

        private void BtnActualiser_Click(object sender, RoutedEventArgs e)
        {
            ChargerDossiers();
        }

        private void BtnVoirDetails_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is Dossier dossier)
            {
                var detailsWindow = new DossierDetailsPage(dossier);
                detailsWindow.ShowDialog();
            }
        }

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is Dossier dossier)
            {
                var detailsWindow = new DossierDetailsPage(dossier);
                if (detailsWindow.ShowDialog() == true)
                {
                    ChargerDossiers();
                }
            }
        }

        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is Dossier dossier)
            {
                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer le dossier {dossier.NumeroIdentite} ?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Dossiers.Remove(dossier);
                        _context.SaveChanges();
                        ChargerDossiers();
                        MessageBox.Show("Dossier supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnNouveauDossier_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.MainContentFrame.Navigate(new PageNouveauDossier());


        }

        private void DgDossiers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Gestion de la sélection si nécessaire
        }
    }
}