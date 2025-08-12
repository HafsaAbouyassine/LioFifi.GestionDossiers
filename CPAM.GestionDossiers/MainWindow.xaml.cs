
using System;
using System.Windows;
using System.Windows.Controls;

namespace CPAM.GestionDossiers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContentFrame.Navigate(new PageAccueil());
        }

        private void BtnAccueil_Click(Object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new PageAccueil());
        }

        private void BtnDossiers_Click(Object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new PageDossiers());
        }

        private void BtnStats_Click(Object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new PageStatistiques());
        }

        private void BtnParametres_Click(Object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new PageParametres());
        }

        private void BtnDeconnexion_Click(Object sender, RoutedEventArgs e)
        {
            // Logic for handling user logout
            MessageBox.Show("Déconnexion réussie !");
            this.Close(); // Close the application or navigate to a login page
        }
    }
}