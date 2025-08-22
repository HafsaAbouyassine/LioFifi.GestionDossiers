using CPAM.GestionDossiers.Data;
using CPAM.GestionDossiers.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;


namespace CPAM.GestionDossiers
{
    /// <summary>
    /// Logique d'interaction pour PageAccueil.xaml
    /// </summary>
    public partial class PageAccueil : Page
    {
        private readonly AppDbContext _context;
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public PageAccueil()
        {
            InitializeComponent();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CPAMDB;Trusted_Connection=True;")
                .Options;
            _context = new AppDbContext(options);


            chargerStatistiques();
        }

        public void chargerStatistiques()
        {
            int dossiersEnCours = _context.Dossiers.Count(d => d.Statut == "En cours");
            int dossiersEnAttente = _context.Dossiers.Count(d => d.Statut == "En attente");
            int dossiersTermines = _context.Dossiers.Count(d => d.Statut == "Terminé");
            var derniereActivite = _context.Dossiers
                .OrderByDescending(d => d.DateDerniereModification)
                .Take(5)
                .ToList();

            txtEnCours.Text = dossiersEnCours.ToString();
            txtEnAttente.Text = dossiersEnAttente.ToString();
            txtTerminer.Text = dossiersTermines.ToString();

            foreach (var dossier in derniereActivite)
            {
                var bloc = new StackPanel { Margin = new Thickness(0, 5, 0, 5) };

                bloc.Children.Add(new TextBlock
                {
                    Text = $"Dossier #{dossier.NumeroIdentite} ({dossier.Statut})",
                    Foreground = new SolidColorBrush(Color.FromRgb(47, 79, 79))
                });

                bloc.Children.Add(new TextBlock
                {
                    Text = $"Modifié le {dossier.DateDerniereModification:dd/MM/yyyy}",
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.FromRgb(127, 159, 191))
                });

                DernieresActivitesPanel.Children.Add(bloc);
            }

        }

        private void BtnNouveauDossier_Click(Object sender, RoutedEventArgs e)
        {
            // Logic to create a new folder
            mainWindow.MainContentFrame.Navigate(new PageNouveauDossier());
        }

        private void RechercherDossier(object sender, RoutedEventArgs e)
        {
            var rechercheWindow = new RechercherDossierWindow(_context);
            rechercheWindow.Owner = Window.GetWindow(this);
            rechercheWindow.ShowDialog();
        }

        public void ImprimerDossier(Dossier dossier)
        {
            // Création du FlowDocument en mémoire
            FlowDocument doc = new FlowDocument();

            // Ajout de titres et paragraphes
            doc.Blocks.Add(new Paragraph(new Run("FICHE DOSSIER")));
            doc.Blocks.Add(new Paragraph(new Run($"N° Identité : {dossier.NumeroIdentite}")));
            doc.Blocks.Add(new Paragraph(new Run($"Nom assuré : {dossier.NomAssure}")));
            doc.Blocks.Add(new Paragraph(new Run($"Statut : {dossier.Statut}")));
            doc.Blocks.Add(new Paragraph(new Run($"Catégorie : {dossier.Categorie}")));
            doc.Blocks.Add(new Paragraph(new Run($"Agent responsable : {dossier.AgentResponsable}")));
            doc.Blocks.Add(new Paragraph(new Run($"Montant : {dossier.MontantRembourse:C}")));
            doc.Blocks.Add(new Paragraph(new Run($"Commentaires : {dossier.Commentaires}")));

            // Boîte de dialogue d’impression Windows
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Impression du dossier");
            }


        }
    }
}
