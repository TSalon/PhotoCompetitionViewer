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
            this.CompOneScoresRequired.IsEnabled = false;

            this.Competition2Button.IsEnabled = false;
            this.CompTwoScoresRequired.IsEnabled = false;

            this.DisableBothHeldButtons();

            List<Competition> competitions = CompetitionHelper.GetCompetitions();

            if (competitions.Count == 0)
            {
                this.StatusLabel.Content = "No competitions found in " + ImagePaths.GetCompetitionsDirectory();
            }
            else
            {
                this.StatusLabel.Content = "Select competition to run:";
                this.Competition1Button.IsEnabled = true;
                this.CompOneScoresRequired.IsEnabled = true;
                this.CompetitionOneBox.Header = competitions[0].GetName();

                if (competitions[0].ScoringEnabled())
                {
                    this.CompOneScoresRequired.Visibility = Visibility.Visible;
                    this.CompOneScorersLabel.Visibility = Visibility.Visible;
                }

                if (competitions.Count > 1)
                {
                    this.CompetitionTwoBox.Header = competitions[1].GetName();
                    this.Competition2Button.IsEnabled = true;
                    this.CompTwoScoresRequired.IsEnabled = true;

                    if (competitions[1].ScoringEnabled())
                    {
                        this.CompTwoScoresRequired.Visibility = Visibility.Visible;
                        this.CompTwoScorersLabel.Visibility = Visibility.Visible;
                    }
                }

                this.RefreshHeldImagesButtons();
            }
        }

        private void DisableBothHeldButtons()
        {
            BitmapImage greySlideshowIcon = new BitmapImage();
            greySlideshowIcon.BeginInit();
            greySlideshowIcon.UriSource = new Uri("pack://application:,,,/Resources/slideshow_grey_72x72.png");
            greySlideshowIcon.EndInit();

            this.Competition1HeldButton.IsEnabled = false;
            this.Competition1SlideshowButton.IsEnabled = false;
            this.Competition1SlideshowImage.Source = greySlideshowIcon;

            this.Competition2HeldButton.IsEnabled = false;
            this.Competition2SlideshowButton.IsEnabled = false;
            this.Competition2SlideshowImage.Source = greySlideshowIcon;

        }

        private void RefreshHeldImagesButtons()
        {
            this.DisableBothHeldButtons();

            List<Competition> competitionNames = CompetitionHelper.GetCompetitions();

            BitmapImage blackSlideshowIcon = new BitmapImage();
            blackSlideshowIcon.BeginInit();
            blackSlideshowIcon.UriSource = new Uri("pack://application:,,,/Resources/slideshow_black_72x72.png");
            blackSlideshowIcon.EndInit();

            int lCompetitionOneHeldImageCount = CompetitionHelper.FetchHeldImageCount(competitionNames[0].GetName());
            this.Competition1HeldButton.IsEnabled = lCompetitionOneHeldImageCount > 0;
            this.Competition1HeldButton.Content = lCompetitionOneHeldImageCount + " Held Images";
            if (this.Competition1HeldButton.IsEnabled)
            {
                this.Competition1SlideshowButton.IsEnabled = true;
                this.Competition1SlideshowImage.Source = blackSlideshowIcon;
            }

            if (competitionNames.Count > 1)
            {
                int lCompetitionTwoHeldImageCount = CompetitionHelper.FetchHeldImageCount(competitionNames[1].GetName());
                this.Competition2HeldButton.IsEnabled = lCompetitionTwoHeldImageCount > 0;
                this.Competition2HeldButton.Content = lCompetitionTwoHeldImageCount + " Held Images";
                if (this.Competition2HeldButton.IsEnabled)
                {
                    this.Competition2SlideshowButton.IsEnabled = true;
                    this.Competition2SlideshowImage.Source = blackSlideshowIcon;
                }
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
