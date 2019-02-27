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
using TPhotoCompetitionViewer.Views.Panel;

namespace TPhotoCompetitionViewer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<IBuzzHandsetDevice> handsets;
        private List<AbstractCompetition> competitionList;

        public MainWindow()
        {
            InitializeComponent();

            this.handsets = new BuzzHandsetFinder().FindHandsets().ToList();

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void LoadCompetitions()
        {
            this.StatusLabel.Content = "Loading competitions...";

            this.Competition1Button.IsEnabled = false;
            this.CompOneScoresRequired.IsEnabled = false;

            this.Competition2Button.IsEnabled = false;
            this.CompTwoScoresRequired.IsEnabled = false;

            this.CompOneScoresRequired.Visibility = Visibility.Hidden;
            this.CompOneScorersLabel.Visibility = Visibility.Hidden;
            this.CompOneScoresButton.Visibility = Visibility.Hidden;
            this.CompTwoScoresRequired.Visibility = Visibility.Hidden;
            this.CompTwoScorersLabel.Visibility = Visibility.Hidden;
            this.CompTwoScoresButton.Visibility = Visibility.Hidden;

            this.DisableBothHeldButtons();

            this.competitionList = CompetitionHelper.GetCompetitions();

            if (this.competitionList.Count == 0)
            {
                this.StatusLabel.Content = "No competitions found in " + ImagePaths.GetCompetitionsDirectory();
            }
            else
            {
                this.StatusLabel.Content = "Select competition to run:";
                this.Competition1Button.IsEnabled = true;
                this.CompOneScoresRequired.IsEnabled = true;
                this.CompetitionOneBox.Header = this.competitionList[0].GetName();

                if (this.competitionList[0].ScoringEnabled())
                {
                    this.CompOneScoresRequired.Visibility = Visibility.Visible;
                    this.CompOneScorersLabel.Visibility = Visibility.Visible;
                    this.CompOneScoresButton.Visibility = Visibility.Visible;
                }

                if (this.competitionList.Count > 1)
                {
                    this.CompetitionTwoBox.Header = this.competitionList[1].GetName();
                    this.Competition2Button.IsEnabled = true;
                    this.CompTwoScoresRequired.IsEnabled = true;

                    if (this.competitionList[1].ScoringEnabled())
                    {
                        this.CompTwoScoresRequired.Visibility = Visibility.Visible;
                        this.CompTwoScorersLabel.Visibility = Visibility.Visible;
                        this.CompTwoScoresButton.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    this.CompetitionTwoBox.Visibility = Visibility.Hidden;
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

            BitmapImage blackSlideshowIcon = new BitmapImage();
            blackSlideshowIcon.BeginInit();
            blackSlideshowIcon.UriSource = new Uri("pack://application:,,,/Resources/slideshow_black_72x72.png");
            blackSlideshowIcon.EndInit();

            int lCompetitionOneHeldImageCount = CompetitionHelper.FetchHeldImageCount(this.competitionList[0].GetName());
            this.Competition1HeldButton.IsEnabled = lCompetitionOneHeldImageCount > 0;
            this.Competition1HeldButton.Content = lCompetitionOneHeldImageCount + " Held";
            if (this.Competition1HeldButton.IsEnabled)
            {
                this.Competition1SlideshowButton.IsEnabled = true;
                this.Competition1SlideshowImage.Source = blackSlideshowIcon;
            }

            if (this.competitionList.Count > 1)
            {
                int lCompetitionTwoHeldImageCount = CompetitionHelper.FetchHeldImageCount(this.competitionList[1].GetName());
                this.Competition2HeldButton.IsEnabled = lCompetitionTwoHeldImageCount > 0;
                this.Competition2HeldButton.Content = lCompetitionTwoHeldImageCount + " Held";
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

        private void RunCompetition(AbstractCompetition competition)
        {
            if (competition is PanelCompetition)
            {
                PanelCompetitionImageWindow competitionPage = new PanelCompetitionImageWindow();
                competitionPage.Init((PanelCompetition)competition, this);
                competitionPage.ShowDialog();
            }
            else
            {
                SingleCompetitionImageWindow competitionPage = new SingleCompetitionImageWindow();
                int scoresRequired = System.Convert.ToInt32(this.CompOneScoresRequired.Text);
                ((Competition)competition).SetScoresRequired(scoresRequired);
                competitionPage.Init((Competition)competition, this.handsets, this);
                competitionPage.ShowDialog();
            }
        }

        private void Competition1Button_Click(object sender, RoutedEventArgs e)
        {
            AbstractCompetition competition = this.competitionList[0];
            this.RunCompetition(competition);
        }

        private void Competition2Button_Click(object sender, RoutedEventArgs e)
        {
            AbstractCompetition competition = this.competitionList[1];
            this.RunCompetition(competition);
        }

        private void ShowHeldImages(AbstractCompetition competition)
        {
            if (competition is PanelCompetition)
            {
                AllHeldPanelsWindow heldPage = new AllHeldPanelsWindow();
                heldPage.Init((PanelCompetition)competition);
                heldPage.ShowDialog();
            }
            else
            {
                AllHeldImagesWindow heldPage = new AllHeldImagesWindow();
                heldPage.Init((Competition)competition);
                heldPage.ShowDialog();
            }
                
        }

        private void Held1Button_Click(object sender, RoutedEventArgs e)
        {
            this.ShowHeldImages(this.competitionList[0]);
        }

        private void Held2Button_Click(object sender, RoutedEventArgs e)
        {
            this.ShowHeldImages(this.competitionList[1]);
        }


        private void Scores1Button_Click(object sender, RoutedEventArgs e)
        {
            ScoresWindow scoresPage = new ScoresWindow();
            scoresPage.Init((Competition)this.competitionList[0], this);
            scoresPage.ShowDialog();
        }

        private void Scores2Button_Click(object sender, RoutedEventArgs e)
        {
            ScoresWindow scoresPage = new ScoresWindow();
            scoresPage.Init((Competition)this.competitionList[1], this);
            scoresPage.ShowDialog();
        }

        private void Slideshow1Button_Click(object sender, RoutedEventArgs e)
        {
            WinnersSlideshowWindow winnersPage = new WinnersSlideshowWindow();
            winnersPage.Init(this.competitionList[0]);
            winnersPage.ShowDialog();
        }

        private void Slideshow2Button_Click(object sender, RoutedEventArgs e)
        {
            WinnersSlideshowWindow winnersPage = new WinnersSlideshowWindow();
            winnersPage.Init(this.competitionList[1]);
            winnersPage.ShowDialog();
        }

        internal void UpdateHeldCount()
        {
            this.RefreshHeldImagesButtons();
        }

        internal void SetupFromCommandLine(string[] args)
        {
            this.LoadCompetitions();
        }
    }
}
