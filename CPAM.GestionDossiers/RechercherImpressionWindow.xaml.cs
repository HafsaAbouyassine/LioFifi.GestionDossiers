using CPAM.GestionDossiers.Data;
using CPAM.GestionDossiers.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CPAM.GestionDossiers
{
    public partial class RechercherImpressionWindow : Window
    {
        private readonly AppDbContext _context;
        private Dossier _dossierTrouve;

        public RechercherImpressionWindow()
        {
            InitializeComponent();
            _context = new AppDbContext();
        }

        private void BtnRechercher_Click(object sender, RoutedEventArgs e)
        {
            EffectuerRecherche();
        }

        private void TxtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnImprimer.Visibility = Visibility.Collapsed;
            resultCard.Visibility = Visibility.Collapsed;
        }

        private void EffectuerRecherche()
        {
            string recherche = txtRecherche.Text.Trim();

            if (string.IsNullOrEmpty(recherche))
            {
                MessageBox.Show("Veuillez entrer un nom ou numéro de dossier.",
                    "Recherche vide", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _dossierTrouve = _context.Dossiers
                    .FirstOrDefault(d => d.NumeroIdentite == recherche ||
                                       d.NomAssure.Contains(recherche));

                if (_dossierTrouve != null)
                {
                    AfficherResultat(_dossierTrouve);
                    btnImprimer.Visibility = Visibility.Visible;
                    resultCard.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Aucun dossier trouvé avec ces critères.",
                        "Non trouvé", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnImprimer.Visibility = Visibility.Collapsed;
                    resultCard.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche: {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AfficherResultat(Dossier dossier)
        {
            txtNumeroDossier.Text = $"Dossier #{dossier.NumeroIdentite}";
            txtNomAssure.Text = dossier.NomAssure;
            txtStatut.Text = dossier.Statut;
            txtDateCreation.Text = $"Créé le {dossier.DateCreation:dd/MM/yyyy}";

            // Couleur du badge selon le statut
            var couleurStatut = dossier.Statut switch
            {
                "Terminé" => "#FF4CAF50", // Vert
                "En cours" => "#FF2196F3", // Bleu
                "En attente" => "#FFFF9800", // Orange
                _ => "#FF607D8B" // Gris
            };

            badgeStatut.Background = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(couleurStatut));
        }

        private void BtnImprimer_Click(object sender, RoutedEventArgs e)
        {
            if (_dossierTrouve == null) return;

            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    ImprimerDocument(printDialog);
                    MessageBox.Show("Impression réussie !", "Succès",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'impression: {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImprimerDocument(PrintDialog printDialog)
        {
            var doc = new FlowDocument
            {
                PagePadding = new Thickness(50),
                FontFamily = new FontFamily("Arial"),
                FontSize = 12,
                TextAlignment = TextAlignment.Center,
                ColumnWidth = double.PositiveInfinity,
                PageWidth = 793
            };

            try
            {
                // === BANNIÈRE EN-TÊTE ===
                var image = new Image();

                //l'image de la bannière
                var uri = new Uri("Icons/BanniereGestionDeDossiers.png", UriKind.Relative);
                image.Source = new BitmapImage(uri);

                // la taille pour l'impression
                image.Width = 1200; // Largeur de la zone imprimable moins les marges
                image.Height = 100; // Hauteur fixe
                image.Stretch = Stretch.Uniform; // Conserver les proportions
                image.HorizontalAlignment = HorizontalAlignment.Center;

                var banniereContainer = new BlockUIContainer(image);
                banniereContainer.Child = image;
                banniereContainer.Margin = new Thickness(0, 0, 0, 40);
                doc.Blocks.Add(banniereContainer);

            }
            catch (Exception ex)
            {
                var headerParagraph = new Paragraph(new Run("LioFifi - GESTION DES DOSSIERS"))
                {
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                doc.Blocks.Add(headerParagraph);
            }

            // Titre du document
            var titleParagraph = new Paragraph(new Run("FICHE DOSSIER"))
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            doc.Blocks.Add(titleParagraph);

            // Séparateur
            doc.Blocks.Add(new Paragraph(new Run(new string('=', 60)))
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15)
            });

            // Informations du dossier dans un tableau formaté
            var table = new Table();
            table.CellSpacing = 0;

            // Colonnes du tableau
            table.Columns.Add(new TableColumn { Width = new GridLength(150) });
            table.Columns.Add(new TableColumn { Width = new GridLength(300) });

            var rowGroup = new TableRowGroup();

            // Ligne 1 - Numéro de dossier
            AddTableRow(rowGroup, "Numéro de dossier:", _dossierTrouve.NumeroIdentite);

            // Ligne 2 - Nom de l'assuré
            AddTableRow(rowGroup, "Nom de l'assuré:", _dossierTrouve.NomAssure);

            // Ligne 3 - Date de création
            AddTableRow(rowGroup, "Date de création:", _dossierTrouve.DateCreation.ToString("dd/MM/yyyy"));

            // Ligne 4 - Statut
            AddTableRow(rowGroup, "Statut:", _dossierTrouve.Statut);

            // Ligne 5 - Catégorie
            AddTableRow(rowGroup, "Catégorie:", _dossierTrouve.Categorie);

            // Ligne 6 - Agent responsable
            AddTableRow(rowGroup, "Agent responsable:", _dossierTrouve.AgentResponsable);

            // Ligne 7 - Montant remboursé (si applicable)
            if (_dossierTrouve.MontantRembourse > 0)
                AddTableRow(rowGroup, "Montant remboursé:", _dossierTrouve.MontantRembourse.ToString("C"));

            // Ligne 8 - Date dernière modification
            AddTableRow(rowGroup, "Dernière modification:", _dossierTrouve.DateDerniereModification.ToString("dd/MM/yyyy HH:mm"));

            table.RowGroups.Add(rowGroup);
            doc.Blocks.Add(table);

            
            if (!string.IsNullOrEmpty(_dossierTrouve.Commentaires))
            {
                doc.Blocks.Add(new Paragraph(new Run("Commentaires:"))
                {
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 20, 0, 5)
                });

                var commentsParagraph = new Paragraph(new Run(_dossierTrouve.Commentaires))
                {
                    Margin = new Thickness(20, 0, 0, 0),
                    FontStyle = FontStyles.Italic,
                    TextAlignment = TextAlignment.Center
                };
                doc.Blocks.Add(commentsParagraph);
            }

            // Séparateur de fin
            doc.Blocks.Add(new Paragraph(new Run(new string('=', 60)))
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 10)
            });

            // Pied de page
            var footerParagraph = new Paragraph(new Run($"Document généré le {DateTime.Now:dd/MM/yyyy à HH:mm} • Système de gestion LioFifi"))
            {
                FontSize = 10,
                TextAlignment = TextAlignment.Center,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 30, 0, 0)
            };
            doc.Blocks.Add(footerParagraph);

            // Impression
            printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator,
                $"Dossier_{_dossierTrouve.NumeroIdentite}");
        }

        private void AddTableRow(TableRowGroup rowGroup, string label, string value)
        {
            var row = new TableRow();

            // Cellule label
            var labelCell = new TableCell(new Paragraph(new Run(label))
            {
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            });
            labelCell.Padding = new Thickness(0, 5, 10, 5);

            // Cellule valeur
            var valueCell = new TableCell(new Paragraph(new Run(value)));
            valueCell.Padding = new Thickness(0, 5, 0, 5);
            valueCell.TextAlignment = TextAlignment.Center;

            row.Cells.Add(labelCell);
            row.Cells.Add(valueCell);
            rowGroup.Rows.Add(row);
        }

        private void BtnFermer_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
