using CPAM.GestionDossiers.Data;
using CPAM.GestionDossiers.Models;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CPAM.GestionDossiers
{
    public partial class PageStatistiques : Page
    {
        private readonly AppDbContext _context;

        // Propriétés pour le binding
        public int TotalDossiers { get; set; }
        public int DossiersTraites { get; set; }
        public int DossiersEnAttente { get; set; }
        public double TauxTraitement { get; set; }

        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection ColumnSeries { get; set; }
        public SeriesCollection LineSeries { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Months { get; set; }

        public PageStatistiques()
        {
            InitializeComponent();
            _context = new AppDbContext();
            Loaded += PageStatistiques_Loaded;
        }

        private void PageStatistiques_Loaded(object sender, RoutedEventArgs e)
        {
            ChargerStatistiques();
        }

        private void ChargerStatistiques()
        {
            try
            {
                var dossiers = _context.Dossiers.ToList();

                // Calcul des indicateurs de base
                TotalDossiers = dossiers.Count;
                DossiersTraites = dossiers.Count(d => d.Statut == "Terminé");
                DossiersEnAttente = dossiers.Count(d => d.Statut == "En attente");
                TauxTraitement = TotalDossiers > 0 ? Math.Round((double)DossiersTraites / TotalDossiers * 100, 1) : 0;

                // Graphique camembert - Répartition par statut
                var statuts = dossiers.GroupBy(d => d.Statut)
                    .Select(g => new { Statut = g.Key, Count = g.Count() })
                    .ToList();

                SeriesCollection = new SeriesCollection();
                foreach (var statut in statuts)
                {
                    SeriesCollection.Add(new PieSeries
                    {
                        Title = statut.Statut,
                        Values = new ChartValues<int> { statut.Count },
                        DataLabels = true
                    });
                }

                // Graphique en barres - Par catégorie
                var categories = dossiers.GroupBy(d => d.Categorie)
                    .Select(g => new { Categorie = g.Key, Count = g.Count() })
                    .ToList();

                ColumnSeries = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Dossiers",
                        Values = new ChartValues<int>(categories.Select(c => c.Count))
                    }
                };

                Categories = categories.Select(c => c.Categorie).ToList();

                // Graphique linéaire - Évolution mensuelle
                var derniersMois = Enumerable.Range(0, 6)
                    .Select(i => DateTime.Now.AddMonths(-i).ToString("MMM yyyy"))
                    .Reverse()
                    .ToList();

                var donneesMensuelles = new List<int>();
                foreach (var mois in derniersMois)
                {
                    var count = dossiers.Count(d =>
                        d.DateCreation.ToString("MMM yyyy") == mois);
                    donneesMensuelles.Add(count);
                }

                LineSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Nouveaux dossiers",
                        Values = new ChartValues<int>(donneesMensuelles),
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 10
                    }
                };

                Months = derniersMois;

                // Performance des agents
                var performanceAgents = dossiers
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
                    .ToList();

                dgAgents.ItemsSource = performanceAgents;

                // Mettre à jour le binding
                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des statistiques: {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnActualiser_Click(object sender, RoutedEventArgs e)
        {
            ChargerStatistiques();
        }

        private void BtnExporter_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = $"Statistiques_LioFifi_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    FlowDocument doc = new FlowDocument
                    {
                        PagePadding = new Thickness(50),
                        FontFamily = new FontFamily("Arial"),
                        FontSize = 12,
                        PageWidth = 793, // Largeur A4 en points (96 dpi)
                        TextAlignment = TextAlignment.Center,
                        ColumnWidth = double.PositiveInfinity
                    };

                    // === BANNIÈRE LioFifi ===
                    try
                    {
                        var image = new Image();

                        // Chargez l'image de la bannière
                        var uri = new Uri("Icons/BanniereGestionDeDossiers.png", UriKind.Relative);
                        image.Source = new BitmapImage(uri);

                        // Ajustez la taille pour le PDF
                        image.Width = 1200; // Largeur adaptée pour A4
                        image.Height = 100;
                        image.Stretch = Stretch.Uniform;
                        image.VerticalAlignment = VerticalAlignment.Top;
                        image.HorizontalAlignment = HorizontalAlignment.Center;

                        var banniereContainer = new BlockUIContainer(image);
                        banniereContainer.Child = image;
                        banniereContainer.Margin = new Thickness(0, 0, 0, 40);
                        doc.Blocks.Add(banniereContainer);

                        // Espace après la bannière
                        doc.Blocks.Add(new Paragraph(new Run(" "))
                        {
                            Margin = new Thickness(0, 15, 0, 0)
                        });
                    }
                    catch
                    {
                        // Fallback si l'image n'est pas trouvée
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

                    // === DATE DU RAPPORT ===
                    var dateRapport = new Paragraph(new Run($"Date du rapport : {DateTime.Now:dd/MM/yyyy à HH:mm}"))
                    {
                        TextAlignment = TextAlignment.Center,
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(0, 0, 0, 30)
                    };
                    doc.Blocks.Add(dateRapport);

                    // === TABLEAU DES INDICATEURS ===
                    var table = new Table
                    {
                        Margin = new Thickness(0, 0, 0, 30),
                        TextAlignment = TextAlignment.Center
                    };
                    table.CellSpacing = 0;

                    // Colonnes du tableau
                    table.Columns.Add(new TableColumn { Width = new GridLength(250) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(150) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(170) });

                    var rowGroup = new TableRowGroup();

                    // En-tête du tableau
                    var headerRow = new TableRow { Background = Brushes.LightGray };
                    AddTableCell(headerRow, "INDICATEUR", true, true);
                    AddTableCell(headerRow, "VALEUR", true, true);
                    AddTableCell(headerRow, "POURCENTAGE", true, true);
                    rowGroup.Rows.Add(headerRow);

                    // Données
                    AddTableRow(rowGroup, "Total des dossiers", TotalDossiers.ToString("N0"), "-");
                    AddTableRow(rowGroup, "Dossiers traités", DossiersTraites.ToString("N0"), $"{((double)DossiersTraites / TotalDossiers * 100):F1}%");
                    AddTableRow(rowGroup, "Dossiers en attente", DossiersEnAttente.ToString("N0"), $"{((double)DossiersEnAttente / TotalDossiers * 100):F1}%");
                    AddTableRow(rowGroup, "Dossiers en cours", (TotalDossiers - DossiersTraites - DossiersEnAttente).ToString("N0"),
                        $"{((double)(TotalDossiers - DossiersTraites - DossiersEnAttente) / TotalDossiers * 100):F1}%");

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
                    doc.Blocks.Add(container);

                    // === GRAPHIQUES (description textuelle pour PDF) ===
                    doc.Blocks.Add(new Paragraph(new Run("SYNTHÈSE GRAPHIQUE"))
                    {
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 30, 0, 15)
                    });

                    // Répartition par statut
                    if (SeriesCollection != null && SeriesCollection.Any())
                    {
                        doc.Blocks.Add(new Paragraph(new Run("Répartition par statut :"))
                        {
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 0, 0, 10)
                        });

                        foreach (var serie in SeriesCollection.OfType<PieSeries>())
                        {
                            doc.Blocks.Add(new Paragraph(new Run($"• {serie.Title} : {serie.Values[0]} dossiers"))
                            {
                                Margin = new Thickness(20, 0, 0, 5)
                            });
                        }
                    }

                    // === PERFORMANCE DES AGENTS ===
                    if (dgAgents.ItemsSource != null)
                    {
                        doc.Blocks.Add(new Paragraph(new Run("PERFORMANCE DES AGENTS"))
                        {
                            FontSize = 16,
                            FontWeight = FontWeights.Bold,
                            TextAlignment = TextAlignment.Center,
                            Margin = new Thickness(0, 30, 0, 15)
                        });

                        var agentTable = new Table();
                        agentTable.CellSpacing = 0;

                        agentTable.Columns.Add(new TableColumn { Width = new GridLength(200) });
                        agentTable.Columns.Add(new TableColumn { Width = new GridLength(100) });
                        agentTable.Columns.Add(new TableColumn { Width = new GridLength(100) });
                        agentTable.Columns.Add(new TableColumn { Width = new GridLength(150) });

                        var agentRowGroup = new TableRowGroup();

                        // En-tête agents
                        var agentHeaderRow = new TableRow { Background = Brushes.LightGray };
                        AddTableCell(agentHeaderRow, "Agent", true, true);
                        AddTableCell(agentHeaderRow, "Traités", true, true);
                        AddTableCell(agentHeaderRow, "En cours", true, true);
                        AddTableCell(agentHeaderRow, "Taux %", true, true);
                        agentRowGroup.Rows.Add(agentHeaderRow);

                        // Données agents
                        foreach (PerformanceAgent agent in dgAgents.ItemsSource)
                        {
                            AddTableRow(agentRowGroup, agent.Agent,
                                agent.DossiersTraites.ToString(),
                                agent.EnCours.ToString(),
                                agent.TauxCompletion.ToString("F1") + "%");
                        }

                        agentTable.RowGroups.Add(agentRowGroup);

                        var container2 = new Paragraph
                        {
                            TextAlignment = TextAlignment.Center
                        };

                        container2.Inlines.Add(new InlineUIContainer(new RichTextBox(new FlowDocument(agentTable))
                        {
                            BorderThickness = new Thickness(0),
                            IsReadOnly = true,
                            Background = Brushes.Transparent,
                            Width = 600 // largeur fixe pour centrer
                        }));
                        doc.Blocks.Add(container2);
                    }
                    
                    // === PIED DE PAGE ===
                    doc.Blocks.Add(new Paragraph(new Run(new string('=', 80)))
                    {
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 30, 0, 10)
                    });

                    var footer = new Paragraph(new Run($"Document généré automatiquement par le système de gestion LioFifi • {DateTime.Now:dd/MM/yyyy HH:mm}"))
                    {
                        FontSize = 10,
                        TextAlignment = TextAlignment.Center,
                        Foreground = Brushes.Gray
                    };
                    doc.Blocks.Add(footer);

                    // === IMPRESSION EN PDF ===
                    var printDialog = new PrintDialog();
                    IDocumentPaginatorSource idpSource = doc;

                    // Utilisez "Microsoft Print to PDF" comme imprimante virtuelle
                    printDialog.PrintDocument(idpSource.DocumentPaginator, "Statistiques LioFifi");

                    MessageBox.Show($"PDF exporté avec succès : {saveFileDialog.FileName}",
                        "Export réussi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'export PDF : {ex.Message}",
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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

    }

    public class PerformanceAgent
    {
        public string Agent { get; set; }
        public int DossiersTraites { get; set; }
        public int EnCours { get; set; }
        public double TauxCompletion { get; set; }
    }
}