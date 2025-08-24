using CPAM.GestionDossiers.Data;
using CPAM.GestionDossiers.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace CPAM.GestionDossiers
{
    /// <summary>
    /// Logique d'interaction pour PageAccueil.xaml
    /// </summary>
    public partial class PageAccueil : Page
    {
        private readonly AppDbContext _context;
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        public double TauxTraitement { get; set; }



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

        private void Imprimer_click(object sender, RoutedEventArgs e)
        {
            var rechercherWindow = new RechercherImpressionWindow();
            rechercherWindow.ShowDialog();
        }

        private void BtnExporter_Click(object sender, RoutedEventArgs e)
        {
            var statistiques = CalculerStatistiques();

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = $"Statistiques_LioFIFI_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            // Premier dialogue pour choisir l'emplacement
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {

                    FlowDocument doc = new FlowDocument
                    {
                        PagePadding = new Thickness(50), // Marge augmentée
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 12,
                        TextAlignment = TextAlignment.Center, // ou Center pour centrer globalement
                        ColumnWidth = double.PositiveInfinity,
                        PageWidth = 793 // Largeur A4
                    };

                    // === BANNIÈRE LioFifi ===
                    try
                    {
                        
                        var image = new Image();

                        // Chemin absolu pour éviter les problèmes
                        var uri = new Uri("pack://application:,,,/Icons/BanniereGestionDeDossiers.png");
                        image.Source = new BitmapImage(uri);

                        image.Width = 1200; // Largeur réduite pour mieux centrer
                        image.Height = 100;
                        image.Stretch = Stretch.Uniform;
                        image.VerticalAlignment = VerticalAlignment.Top;
                        image.HorizontalAlignment = HorizontalAlignment.Center;

                        var banniereContainer = new BlockUIContainer(image);
                        banniereContainer.Child = image;
                        banniereContainer.Margin = new Thickness(0, 0, 0, 40);
                        doc.Blocks.Add(banniereContainer);

                    }
                    catch
                    {
                        // Fallback textuel
                        var headerParagraph = new Paragraph(new Run("LioFifi - GESTION DES DOSSIERS"))
                        {
                            FontSize = 18,
                            FontWeight = FontWeights.Bold,
                            TextAlignment = TextAlignment.Center,
                            Margin = new Thickness(0, 0, 0, 20)
                        };
                        doc.Blocks.Add(headerParagraph);
                    }

                    // === TITRE PRINCIPAL ===
                    var titre = new Paragraph(new Run("RAPPORT STATISTIQUES - LioFifi DE VAUCLUSE"))
                    {
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                    doc.Blocks.Add(titre);

                    // === CONTENU PRINCIPAL ===
                    var section = new Section();

                    // Tableau des indicateurs
                    section.Blocks.Add(CreerTableauIndicateurs(statistiques));

                    // Répartition par statut
                    section.Blocks.Add(CreerSectionRepartition("RÉPARTITION PAR STATUT", statistiques.RepartitionStatuts, statistiques.TotalDossiers));

                    // Répartition par catégorie
                    section.Blocks.Add(CreerSectionRepartition("RÉPARTITION PAR CATÉGORIE", statistiques.RepartitionCategories, statistiques.TotalDossiers));

                    // Performance des agents
                    section.Blocks.Add(CreerTableauAgents(statistiques.PerformanceAgents));

                    doc.Blocks.Add(section);

                    // === PIED DE PAGE ===
                    var footer = new Paragraph(new Run($"Document généré le {DateTime.Now:dd/MM/yyyy à HH:mm} • Système de gestion LioFifi"))
                    {
                        FontSize = 10,
                        TextAlignment = TextAlignment.Center,
                        Foreground = Brushes.Gray,
                        Margin = new Thickness(0, 30, 0, 0)
                    };
                    doc.Blocks.Add(footer);

                    // === IMPRESSION ===
                    // Dialogue d'impression UNIQUEMENT pour choisir l'imprimante PDF
                    PrintDialog printDialog = new PrintDialog();

                    // Configurez pour utiliser "Microsoft Print to PDF"
                    if (printDialog.ShowDialog() == true)
                    {
                        printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator,
                            "Statistiques LioFifi");

                        MessageBox.Show($"PDF exporté avec succès : {saveFileDialog.FileName}",
                            "Export réussi", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'export PDF : {ex.Message}",
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Méthode pour calculer les statistiques
        private StatistiquesData CalculerStatistiques()
        {
            using (var context = new AppDbContext())
            {
                var dossiers = context.Dossiers.ToList();

                var statistiques = new StatistiquesData
                {
                    TotalDossiers = dossiers.Count,
                    DossiersTraites = dossiers.Count(d => d.Statut == "Terminé"),
                    DossiersEnAttente = dossiers.Count(d => d.Statut == "En attente"),
                    DossiersEnCours = dossiers.Count(d => d.Statut == "En cours"),

                    RepartitionStatuts = dossiers.GroupBy(d => d.Statut)
                        .ToDictionary(g => g.Key, g => g.Count()),

                    RepartitionCategories = dossiers.GroupBy(d => d.Categorie)
                        .ToDictionary(g => g.Key, g => g.Count()),

                    PerformanceAgents = dossiers
                        .Where(d => !string.IsNullOrEmpty(d.AgentResponsable))
                        .GroupBy(d => d.AgentResponsable)
                        .Select(g => new PerformanceAgent
                        {
                            Agent = g.Key,
                            DossiersTraites = g.Count(d => d.Statut == "Terminé"),
                            EnCours = g.Count(d => d.Statut == "En cours"),
                            TauxCompletion = g.Count() > 0 ?
                                Math.Round((double)g.Count(d => d.Statut == "Terminé") / g.Count() * 100, 1) : 0
                        })
                        .OrderByDescending(p => p.DossiersTraites)
                        .ToList()
                };

                return statistiques;
            }
        }

        // Classes de données
        public class StatistiquesData
        {
            public int TotalDossiers { get; set; }
            public int DossiersTraites { get; set; }
            public int DossiersEnAttente { get; set; }
            public int DossiersEnCours { get; set; }
            public Dictionary<string, int> RepartitionStatuts { get; set; }
            public Dictionary<string, int> RepartitionCategories { get; set; }
            public List<PerformanceAgent> PerformanceAgents { get; set; }
        }

        public class PerformanceAgent
        {
            public string Agent { get; set; }
            public int DossiersTraites { get; set; }
            public int EnCours { get; set; }
            public double TauxCompletion { get; set; }
        }

        // Méthodes helper pour les tableaux
        private void AddTableCell(TableRow row, string text, bool isBold = false, bool isCentered = false)
        {
            var paragraph = new Paragraph(new Run(text));
            if (isBold) paragraph.FontWeight = FontWeights.Bold;
            if (isCentered) paragraph.TextAlignment = TextAlignment.Center;

            var cell = new TableCell(paragraph)
            {
                Padding = new Thickness(8, 4, 8, 4),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 1, 1)
            };

            row.Cells.Add(cell);
        }

        private void AddTableRow(TableRowGroup rowGroup, params string[] values)
        {
            var row = new TableRow();
            foreach (var value in values)
            {
                AddTableCell(row, value);
            }
            rowGroup.Rows.Add(row);
        }

        private Block CreerTableauIndicateurs(StatistiquesData statistiques)
        {
            var table = new Table
            {
                Margin = new Thickness(0, 0, 0, 30),
                TextAlignment = TextAlignment.Center
            };

            // Colonnes
            table.Columns.Add(new TableColumn { Width = new GridLength(250) });
            table.Columns.Add(new TableColumn { Width = new GridLength(150) });
            table.Columns.Add(new TableColumn { Width = new GridLength(170) });

            var rowGroup = new TableRowGroup();

            // En-tête
            var headerRow = new TableRow { Background = Brushes.LightGray };
            AddTableCell(headerRow, "INDICATEUR", true, true);
            AddTableCell(headerRow, "VALEUR", true, true);
            AddTableCell(headerRow, "POURCENTAGE", true, true);
            rowGroup.Rows.Add(headerRow);

            // Données
            AddTableRow(rowGroup, "Total des dossiers", statistiques.TotalDossiers.ToString("N0"), "-");
            AddTableRow(rowGroup, "Dossiers traités", statistiques.DossiersTraites.ToString("N0"),
                GetPourcentage(statistiques.DossiersTraites, statistiques.TotalDossiers));

            table.RowGroups.Add(rowGroup);

            // 💡 Encapsulation dans un Paragraph pour centrer
            var container = new Paragraph
            {
                TextAlignment = TextAlignment.Center
            };
            container.Inlines.Add(new InlineUIContainer(new RichTextBox(new FlowDocument(table))
            {
                BorderThickness = new Thickness(0),
                IsReadOnly = true,
                Background = Brushes.Transparent,
                Width = 600 // largeur fixe pour centrer
            }));

            return container;
        }



        private Block CreerTableauAgents(List<PerformanceAgent> agents)
        {
            if (agents.Any())
            {
                var table = new Table
                {
                    Margin = new Thickness(0, 0, 0, 20),
                    TextAlignment = TextAlignment.Center
                };

                // Colonnes
                table.Columns.Add(new TableColumn { Width = new GridLength(200) });
                table.Columns.Add(new TableColumn { Width = new GridLength(100) });
                table.Columns.Add(new TableColumn { Width = new GridLength(100) });
                table.Columns.Add(new TableColumn { Width = new GridLength(200) });


                var rowGroup = new TableRowGroup();

                // En-tête
                var headerRow = new TableRow { Background = Brushes.LightGray };
                AddTableCell(headerRow, "Agent", true, true);
                AddTableCell(headerRow, "Traités", true, true);
                AddTableCell(headerRow, "En cours", true, true);
                AddTableCell(headerRow, "Taux %", true, true);
                rowGroup.Rows.Add(headerRow);
               
                foreach (var agent in agents)
                {
                    AddTableRow(rowGroup, agent.Agent,
                        agent.DossiersTraites.ToString(),
                        agent.EnCours.ToString(),
                        agent.TauxCompletion.ToString("F1") + "%");
                }
                table.RowGroups.Add(rowGroup);

                var container = new Paragraph
                {
                    TextAlignment = TextAlignment.Center
                };

                container.Inlines.Add(new InlineUIContainer(new RichTextBox(new FlowDocument(table))
                {
                    BorderThickness = new Thickness(0),
                    IsReadOnly = true,
                    Background = Brushes.Transparent,
                    Width = 600 // largeur fixe pour centrer
                }));

                return container; // ✅ retourne bien le tableau
            }
            else
            {
                // Si aucun agent, on retourne un Paragraph (qui est un Block)
                return new Paragraph(new Run("Aucune donnée d'agent disponible"))
                {
                    FontStyle = FontStyles.Italic,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(10, 0, 0, 20)
                };
            }
        }


        private Block CreerSectionRepartition(string titre, Dictionary<string, int> donnees, int total)
        {
            var section = new Section();

            section.Blocks.Add(new Paragraph(new Run(titre))
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15)
            });

            foreach (var item in donnees.OrderByDescending(x => x.Value))
            {
                section.Blocks.Add(new Paragraph(new Run(
                    $"• {item.Key} : {item.Value} dossiers ({GetPourcentage(item.Value, total)})"))
                {
                    Margin = new Thickness(20, 0, 0, 5)
                });
            }

            section.Margin = new Thickness(0, 0, 0, 30);
            return section;
        }

        

        private string GetPourcentage(int valeur, int total)
        {
            return total > 0 ? $"{((double)valeur / total * 100):F1}%" : "0%";
        }

    }
}
