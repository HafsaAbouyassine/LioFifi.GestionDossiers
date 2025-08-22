using CPAM.GestionDossiers.Data;
using CPAM.GestionDossiers.Models;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            MessageBox.Show("Fonctionnalité d'export PDF à implémenter",
                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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