﻿using BuzzIO;
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
        private bool playAudio = true;
        private bool showCursor = false;

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

            Properties.Settings.Default.Reload();
            this.setCursorState(Properties.Settings.Default.ShowCursor);
            this.setAudioState(Properties.Settings.Default.PlayAudioTitles);
        }

        private void DisableBothHeldButtons()
        {
            BitmapImage greySlideshowIcon = new BitmapImage();
            greySlideshowIcon.BeginInit();
            greySlideshowIcon.UriSource = new Uri("pack://application:,,,/Resources/slideshow_grey_72x72.png");
            greySlideshowIcon.EndInit();

            BitmapImage greyUploadIcon = new BitmapImage();
            greyUploadIcon.BeginInit();
            greyUploadIcon.UriSource = new Uri("pack://application:,,,/Resources/file_upload_grey_72x72.png");
            greyUploadIcon.EndInit();

            this.Competition1HeldButton.IsEnabled = false;
            this.Competition1SlideshowButton.IsEnabled = false;
            this.Competition1SlideshowImage.Source = greySlideshowIcon;
            this.Competition1UploadButton.IsEnabled = false;
            this.Competition1UploadImage.Source = greyUploadIcon;

            this.Competition2HeldButton.IsEnabled = false;
            this.Competition2SlideshowButton.IsEnabled = false;
            this.Competition2SlideshowImage.Source = greySlideshowIcon;
            this.Competition2UploadButton.IsEnabled = false;
            this.Competition2UploadImage.Source = greyUploadIcon;

        }

        private void RefreshHeldImagesButtons()
        {
            this.DisableBothHeldButtons();

            BitmapImage blackSlideshowIcon = new BitmapImage();
            blackSlideshowIcon.BeginInit();
            blackSlideshowIcon.UriSource = new Uri("pack://application:,,,/Resources/slideshow_black_72x72.png");
            blackSlideshowIcon.EndInit();

            BitmapImage blackUploadIcon = new BitmapImage();
            blackUploadIcon.BeginInit();
            blackUploadIcon.UriSource = new Uri("pack://application:,,,/Resources/file_upload_black_72x72.png");
            blackUploadIcon.EndInit();

            int lCompetitionOneHeldImageCount = CompetitionHelper.FetchHeldImageCount(this.competitionList[0].GetName());
            this.Competition1HeldButton.IsEnabled = lCompetitionOneHeldImageCount > 0;
            this.Competition1HeldButton.Content = lCompetitionOneHeldImageCount + " Held";
            if (this.Competition1HeldButton.IsEnabled)
            {
                this.Competition1SlideshowButton.IsEnabled = true;
                this.Competition1SlideshowImage.Source = blackSlideshowIcon;

                int lCompetitionOneAwardedImageCount = CompetitionHelper.FetchAwardedImageCount(this.competitionList[0].GetName());
                if (lCompetitionOneAwardedImageCount > 0)
                {
                    this.Competition1UploadButton.IsEnabled = true;
                    this.Competition1UploadImage.Source = blackUploadIcon;
                }

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

                    int lCompetitionTwoAwardedImageCount = CompetitionHelper.FetchAwardedImageCount(this.competitionList[1].GetName());
                    if(lCompetitionTwoAwardedImageCount > 0)
                    {
                        this.Competition2UploadButton.IsEnabled = true;
                        this.Competition2UploadImage.Source = blackUploadIcon;
                    }
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
                if (this.IsCursorEnabled())
                {
                    competitionPage.Cursor = Cursors.Arrow;
                }
                competitionPage.Init((PanelCompetition)competition, this);
                competitionPage.ShowDialog();
            }
            else
            {
                SingleCompetitionImageWindow competitionPage = new SingleCompetitionImageWindow();
                if (this.IsCursorEnabled())
                {
                    competitionPage.Cursor = Cursors.Arrow;
                }
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

        private void ToggleAudioButton_Click(object sender, RoutedEventArgs e)
        {
            this.setAudioState(!this.playAudio);
        }

        private void setAudioState(bool audioState) {
            this.playAudio = audioState;
            Properties.Settings.Default.PlayAudioTitles = this.playAudio;
            Properties.Settings.Default.Save();
            if (audioState)
            {
                BitmapImage playAudioTitlesIcon = new BitmapImage();
                playAudioTitlesIcon.BeginInit();
                playAudioTitlesIcon.UriSource = new Uri("pack://application:,,,/Resources/speaker_notes_black_72x72.png");
                playAudioTitlesIcon.EndInit();
                this.MuteButtonImage.Source = playAudioTitlesIcon;
            }
            else
            {
                BitmapImage muteAudioTitlesIcon = new BitmapImage();
                muteAudioTitlesIcon.BeginInit();
                muteAudioTitlesIcon.UriSource = new Uri("pack://application:,,,/Resources/speaker_notes_off_black_72x72.png");
                muteAudioTitlesIcon.EndInit();
                this.MuteButtonImage.Source = muteAudioTitlesIcon;
            }
        }


        private void ToggleCursorButton_Click(object sender, RoutedEventArgs e)
        {
            this.setCursorState(!this.showCursor);
        }
        private void setCursorState(bool cursorState) 
        {
            this.showCursor = cursorState;
            Properties.Settings.Default.ShowCursor = this.showCursor;
            Properties.Settings.Default.Save();
            if (cursorState)
            {
                BitmapImage showCursorIcon = new BitmapImage();
                showCursorIcon.BeginInit();
                showCursorIcon.UriSource = new Uri("pack://application:,,,/Resources/visibility_black_72x72.png");
                showCursorIcon.EndInit();
                this.ShowCursorButtonImage.Source = showCursorIcon;
            }
            else
            {
                BitmapImage hideCursorIcon = new BitmapImage();
                hideCursorIcon.BeginInit();
                hideCursorIcon.UriSource = new Uri("pack://application:,,,/Resources/visibility_off_black_72x72.png");
                hideCursorIcon.EndInit();
                this.ShowCursorButtonImage.Source = hideCursorIcon;
            }
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
            this.UpdateHeldCount();
        }

        private void Held2Button_Click(object sender, RoutedEventArgs e)
        {
            this.ShowHeldImages(this.competitionList[1]);
            this.UpdateHeldCount();
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

        private void Upload1Button_Click(object sender, RoutedEventArgs e)
        {
            ResultsUploadWindow winnersPage = new ResultsUploadWindow();
            winnersPage.Init(this.competitionList[0]);
            winnersPage.ShowDialog();
        }

        internal bool IsAudioEnabled()
        {
            return this.playAudio;
        }

        internal bool IsCursorEnabled()
        {
            return this.showCursor;
        }

        private void Upload2Button_Click(object sender, RoutedEventArgs e)
        {
            ResultsUploadWindow winnersPage = new ResultsUploadWindow();
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
