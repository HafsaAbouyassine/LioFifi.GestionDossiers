using CPAM.GestionDossiers.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using CPAM.GestionDossiers.Models;
using System.Text.RegularExpressions;

namespace CPAM.GestionDossiers
{
    /// <summary>
    /// Logique d'interaction pour PageNouveauDossier.xaml
    /// </summary>
    public partial class PageNouveauDossier : Page
    {
        private readonly AppDbContext _context;
        public PageNouveauDossier()
        {
            InitializeComponent();
            var opions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CPAMDB;Trusted_Connection=True;")
                .Options;
            _context = new AppDbContext(opions);
        }

        // ...

        private void SaveNewFolder(object sender, RoutedEventArgs e)
        {
            string numeroIdentite = txtNumeroDossier.Text?.Trim();

            //Regex pour numéro de nationalité (13 chiffres) ou titre de séjour (10 alphanumérique)
            var regexNationalite = new Regex(@"^\d{13}$");
            var regexTitreSejour = new Regex(@"^[A-Za-z0-9]{10}$");

            if(!regexTitreSejour.IsMatch(numeroIdentite) && !regexNationalite.IsMatch(numeroIdentite))
            {
                MessageBox.Show("Le numéro d'identité doit être un numéro de nationalité (13 chiffres) ou un titre de séjour (10 alphanumérique).", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                txtNumeroDossier.Focus();
                return;
            }

            var dossier = new Dossier
            {
                NumeroIdentite = numeroIdentite,
                NomAssure = txtNomBeneficiaire.Text,
                DateCreation = DateTime.Now,
                DateNaissance = dateNaissance.SelectedDate,
                Statut = (status.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                Categorie = (category.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                AgentResponsable = (agentResponsable.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                DateDerniereModification = DateTime.Now
            };

            _context.Dossiers.Add(dossier);
            _context.SaveChanges();

            MessageBox.Show("Dossier ajouté avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

            NavigationService?.Navigate(new PageAccueil());
        }

        private void CancelNewFolder(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Êtes-vous sûr de vouloir annuler la création du dossier ?", "Annuler", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                NavigationService?.Navigate(new PageAccueil());

            }
        }
    }
}
