using BuzzIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TPhotoCompetitionViewer.Competitions;

namespace TPhotoCompetitionViewer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<IBuzzHandsetDevice> handsets;

        public MainWindow()
        {
            InitializeComponent();

            this.handsets = new BuzzHandsetFinder().FindHandsets().ToList();

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            this.LoadCompetitions();
        }

        private void LoadCompetitions()
        {
            this.StatusLabel.Content = "Loading competitions...";

            this.Competition1Button.IsEnabled = false;
            this.Competition1HeldButton.IsEnabled = false;
            this.Competition1SlideshowButton.IsEnabled = false;
            this.CompOneScoresRequired.IsEnabled = false;

            this.Competition2Button.IsEnabled = false;
            this.Competition2HeldButton.IsEnabled = false;
            this.Competition2SlideshowButton.IsEnabled = false;
            this.CompTwoScoresRequired.IsEnabled = false;
            List<string> competitionNames = CompetitionHelper.GetCompetitions();

            if (competitionNames.Count == 0)
            {
                this.StatusLabel.Content = "No competitions found in " + ImagePaths.GetCompetitionsDirectory();
            }
            else
            {
                this.StatusLabel.Content = "Select competition to run:";
                this.Competition1Button.IsEnabled = true;
                this.CompOneScoresRequired.IsEnabled = true;
                this.CompetitionOneBox.Header = competitionNames[0];

                if (competitionNames.Count > 1)
                {
                    this.CompetitionTwoBox.Header = competitionNames[1];
                    this.Competition2Button.IsEnabled = true;
                    this.CompTwoScoresRequired.IsEnabled = true;
                }

                this.RefreshHeldImagesButtons();
            }
        }

        private void RefreshHeldImagesButtons()
        {
            List<string> competitionNames = CompetitionHelper.GetCompetitions();

            int lCompetitionOneHeldImageCount = CompetitionHelper.FetchHeldImageCount(competitionNames[0]);
            this.Competition1HeldButton.IsEnabled = lCompetitionOneHeldImageCount > 0;
            this.Competition1HeldButton.Content = lCompetitionOneHeldImageCount + " Held Images";
            this.Competition1SlideshowButton.IsEnabled = this.Competition1HeldButton.IsEnabled;

            if (competitionNames.Count > 1)
            {
                int lCompetitionTwoHeldImageCount = CompetitionHelper.FetchHeldImageCount(competitionNames[1]);
                this.Competition2HeldButton.IsEnabled = lCompetitionTwoHeldImageCount > 0;
                this.Competition2HeldButton.Content = lCompetitionTwoHeldImageCount + " Held Images";
                this.Competition2SlideshowButton.IsEnabled = this.Competition2HeldButton.IsEnabled;
            }
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Competition1Button_Click(object sender, RoutedEventArgs e)
        {
            int scoresRequired = System.Convert.ToInt32(this.CompOneScoresRequired.Text);
            SingleCompetitionImageWindow competitionPage = new SingleCompetitionImageWindow();
            Competition competition = CompetitionHelper.GetCompetition(0, scoresRequired);
            competitionPage.Init(competition, this.handsets, this);
            competitionPage.ShowDialog();
        }

        private void Competition2Button_Click(object sender, RoutedEventArgs e)
        {
            int scoresRequired = System.Convert.ToInt32(this.CompTwoScoresRequired.Text);
            SingleCompetitionImageWindow competitionPage = new SingleCompetitionImageWindow();
            Competition competition = CompetitionHelper.GetCompetition(1, scoresRequired);
            competitionPage.Init(competition, this.handsets, this);
            competitionPage.ShowDialog();
        }

        private void Held1Button_Click(object sender, RoutedEventArgs e)
        {
            AllHeldImagesWindow heldPage = new AllHeldImagesWindow();
            heldPage.Init(0);
            heldPage.ShowDialog();
        }

        private void Held2Button_Click(object sender, RoutedEventArgs e)
        {
            AllHeldImagesWindow heldPage = new AllHeldImagesWindow();
            heldPage.Init(1);
            heldPage.ShowDialog();
        }

        private void Slideshow1Button_Click(object sender, RoutedEventArgs e)
        {
            WinnersSlideshowWindow winnersPage = new WinnersSlideshowWindow();
            winnersPage.Init(0);
            winnersPage.ShowDialog();
        }

        private void Slideshow2Button_Click(object sender, RoutedEventArgs e)
        {
            WinnersSlideshowWindow winnersPage = new WinnersSlideshowWindow();
            winnersPage.Init(1);
            winnersPage.ShowDialog();
        }

        internal void UpdateHeldCount()
        {
            this.RefreshHeldImagesButtons();
        }
    }
}
