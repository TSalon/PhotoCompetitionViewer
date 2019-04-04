using BuzzIO;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TPhotoCompetitionViewer.Competitions;
using TPhotoCompetitionViewer.Handsets;
using TPhotoCompetitionViewer.Views;

namespace TPhotoCompetitionViewer.Views
{
    /// <summary>
    /// Interaction logic for SingleCompetitionImageWindow.xaml
    ///   - Responsible for handling all interaction with scoring controllers
    /// </summary>
    public partial class SingleCompetitionImageWindow : Window
    {
        private enum PageMode { Start, Image, End };
        private Competition competition;
        private CompetitionImage competitionImage;
        private int imageIndex = 0;
        private DispatcherTimer titleTimer;
	    private DispatcherTimer enableScoringTimer;
        private MainWindow mainWindow;
        private HandsetWrapper handsets;
        private SQLiteConnection dbConnection;
	    private bool scoringEnabled = false;
        private bool titleShown = false;
        private PageMode currentMode = PageMode.Start;

        /** Initialise Window */
        public SingleCompetitionImageWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleKeys);

            //Create a timer with interval of 5 secs for showing the title
            this.titleTimer = new DispatcherTimer();
            this.titleTimer.Tick += new EventHandler(TitleTimer_Tick);
            this.titleTimer.Interval = new TimeSpan(0, 0, TimingValues.TIME_TO_SHOW_TITLES_FOR_SECONDS);

	        //Create a timer with interval of 2 second to prevent immediate scoring
	        this.enableScoringTimer = new DispatcherTimer();
	        this.enableScoringTimer.Tick += new EventHandler(ScoringTimer_Tick);
	        this.enableScoringTimer.Interval = new TimeSpan(0,0, TimingValues.SCORING_DELAY_SECONDS);
        }


        /** Initialise this window for a particular competition and show the title page */
        internal void Init(Competition competition, List<IBuzzHandsetDevice> handsets, MainWindow mainWindow)
        {
            // Get handle to main window
            this.mainWindow = mainWindow;

            // get handle to buzz controllers and register event handler
            this.handsets = new HandsetWrapper(handsets);
            if (this.handsets.HasHandsets())
            {
                this.handsets.Get(0).ButtonChanged += HandleHandsetEvent0;
                if (this.handsets.HasSecondHandsetGroup())
                {
                    this.handsets.Get(1).ButtonChanged += HandleHandsetEvent1;
                }

                // start with all handset lights turned off
                this.handsets.AllLightsOff();
            }

            this.competition = competition;

            // Get a handle to the database for this competition
            string databaseFilePath = ImagePaths.GetDatabaseFile(this.competition.GetName());
            this.dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");

            this.SetPageMode(PageMode.Start);
        }

        private void HideAllControls()
        {
            this.ImagePane.Visibility = Visibility.Hidden;
            this.ImageTitle.Visibility = Visibility.Hidden;
            this.ImagePosition.Visibility = Visibility.Hidden;
            this.ClubNameLabel.Visibility = Visibility.Hidden;
            this.CompetitionNameLabel.Visibility = Visibility.Hidden;
            this.TrophyNameLabel.Visibility = Visibility.Hidden;

            this.titleShown = false;

            this.titleTimer.Stop();
            this.enableScoringTimer.Stop();

            this.scoringEnabled = false;
        }

        private void SetPageMode(PageMode pageMode)
        {
            this.currentMode = pageMode;

            switch (this.currentMode)
            {
                case PageMode.Start:
                    this.HideAllControls();

                    this.ClubNameLabel.Visibility = Visibility.Visible;
                    this.ClubNameLabel.Content = this.competition.GetClubName();
                    this.CompetitionNameLabel.Visibility = Visibility.Visible;
                    string competitionName = this.competition.GetName();
                    string competitionNameLessDate = this.competition.GetName().Substring(11);
                    this.CompetitionNameLabel.Content = competitionNameLessDate;
                    this.TrophyNameLabel.Visibility = Visibility.Visible;
                    this.TrophyNameLabel.Content = this.competition.GetTrophyName();

                    break;

                case PageMode.Image:
                    this.HideAllControls();

                    this.ImagePane.Visibility = Visibility.Visible;

                    break;

                case PageMode.End:
                    this.HideAllControls();

                    this.CompetitionNameLabel.Visibility = Visibility.Visible;
                    this.CompetitionNameLabel.Content = "That's All Folks!";
                    
                    break;
                
            }
        }


        /** Handle timer tick to hide image title */
        private void TitleTimer_Tick(object sender, EventArgs e)
        {
           this.ImageTitle.Visibility = Visibility.Hidden;
           this.ImagePosition.Visibility = Visibility.Hidden;

            this.titleShown = false;

            this.titleTimer.IsEnabled = false;
        }

	    /** Handler timer tick to enable scoring */
	    private void ScoringTimer_Tick(object sender, EventArgs e)
	    {
	      this.scoringEnabled = true;
	    }


        /** Event handler for first handset group */
        private void HandleHandsetEvent0(object sender, BuzzButtonChangedEventArgs e)
        {
            this.HandleHandsetEvent(0, e);         
        }

        /** Event handler for second handset group, if present */
        private void HandleHandsetEvent1(object sender, BuzzButtonChangedEventArgs e)
        {
            this.HandleHandsetEvent(1, e);
        }

        /** Handle the click of a handset button */
        private void HandleHandsetEvent(int handsetGroupNumber, BuzzButtonChangedEventArgs e)
        {
            for (int i=0; i<e.Buttons.Length;i++)
            {
                var eachHandsetButtons = e.Buttons[i];
                if (eachHandsetButtons.Any)
                {
                    var handsetId = HandsetIdTools.BuildHandsetId(handsetGroupNumber, i);
                    if (eachHandsetButtons.Blue) { this.ScoreImage(handsetId, 5); continue; }
                    if (eachHandsetButtons.Orange) { this.ScoreImage(handsetId, 4); continue; }
                    if (eachHandsetButtons.Green) { this.ScoreImage(handsetId, 3); continue; }
                    if (eachHandsetButtons.Yellow) { this.ScoreImage(handsetId, 2); continue; }
                    if (eachHandsetButtons.Red) { this.ScoreImage(handsetId, 0); continue; }
                }
            }
        }

        /** Record the score associated with a pushed handset button */
        private void ScoreImage(string handsetId, int score)
        {
            if (this.competitionImage != null && this.scoringEnabled)
            {
                int totalScore = this.competitionImage.ScoreImage(handsetId, score, this.dbConnection);
                this.handsets.SetLightsForThisImage(this.competitionImage);

                if (totalScore > 0)
                {
                    // we have total score
                    this.Dispatcher.Invoke(() =>
                    {
                        // Wait a bit, so that humans don't think we're reading their tiny little minds
                        Thread.Sleep(TimingValues.DELAY_BEFORE_READING_SCORE_MS);

                        // Read out the appropriate score
                        this.MediaElement.Source = new Uri("Resources/Numbers/Brian/" + totalScore + ".mp3", UriKind.Relative);
                        this.MediaElement.Play();

                        // allow time for number to be read out and human perception time
                        Thread.Sleep(TimingValues.DELAY_BEFORE_SHOWING_NEXT_IMAGE_MS);

                        // Move to next image
                        this.NextImage(false);
                    });
                }
            }
        }

        /** Handle a key on the keyboard being pushed */
        private void HandleKeys(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.CloseWindow();
                    break;

                case Key.Right:
                    this.NextImage(false);
                    break;

                case Key.PageDown: // clicker right
                    this.NextImage(true);
                    break;

                case Key.Left:
                    this.PreviousImage(false);
                    break;

                case Key.PageUp: // clicker left
                    this.PreviousImage(true);
                    break;

                case Key.T:
                case Key.B: // clicker stop button
                    this.ShowTitle();
                    break;

                case Key.H:
                    this.HoldImage();
                    break;
            }
        }

        private void CloseWindow()
        {
            this.handsets.AllLightsOff();
            this.Close();
        }

        /** Mark an image as held */
        private void HoldImage()
        {
            if (this.currentMode != PageMode.Image)
            {
                return;
            }
            
            this.competitionImage.ToggleHeld(this.dbConnection);
            this.ShowTitle();

            this.mainWindow.UpdateHeldCount();
        }

        /** Show the image at the specified index */
        private void ShowImage(int imageIndex)
        {
	        this.titleTimer.Stop();
	
	        this.scoringEnabled = false;
	        this.enableScoringTimer.Stop();


            this.imageIndex = imageIndex;
            this.competitionImage = this.competition.GetImageObject(imageIndex);

            this.handsets.SetLightsForThisImage(this.competitionImage);

            string imagePath = this.competitionImage.GetFullFilePath();
            BitmapImage imageToShow = new BitmapImage();
            imageToShow.BeginInit();
            imageToShow.UriSource = new Uri(imagePath);
            imageToShow.EndInit();
            this.ImagePane.Source = imageToShow;

            this.ShowTitle();
        }

        /** Show the image title for a short period of time */
        private void ShowTitle()
        {
            if (this.currentMode != PageMode.Image)
            {
                return;
            }

            string imageName = this.competitionImage.GetTitle();
            int imageCount = this.competition.MaxImageIndex() + 1;
            int imageCurrentIndex = this.imageIndex + 1;
            string imagePosition;

            if (this.competitionImage.IsHeld(this.dbConnection))
            {
                imagePosition = "[" + imageCurrentIndex + "/" + imageCount + "]" ;
            }
            else
            {
                imagePosition = "(" + imageCurrentIndex + "/" + imageCount + ")";
            }

            this.ImageTitle.Content = imageName;
            this.ImagePosition.Content = imagePosition;
            this.ImageTitle.Visibility = Visibility.Visible;
            this.ImagePosition.Visibility = Visibility.Visible;

            this.titleShown = true;

            this.titleTimer.Start();
	        this.enableScoringTimer.Start();

            string lMp3Path = ImagePaths.GetExtractDirectory(this.competition.GetName()) + "/" + this.competitionImage.GetAudioPath();
            this.MediaElement.Source = new Uri(lMp3Path, UriKind.Absolute);
            this.MediaElement.Play();
        }

        /** Show the next image */
        private void NextImage(Boolean waitForTitleToDisappear)
        {
            if (waitForTitleToDisappear && this.titleShown)
            {
                return;
            }

            switch (this.currentMode)
            {
                case PageMode.Start:
                    this.SetPageMode(PageMode.Image);
                    // Show first image
                    this.imageIndex = 0;
                    this.ShowImage(this.imageIndex);
                    break;

                case PageMode.Image:
                    // Show next image, or end page
                    if (this.imageIndex < this.competition.MaxImageIndex())
                    {
                        this.ShowImage(this.imageIndex + 1);
                    }
                    else
                    {
                        this.SetPageMode(PageMode.End);

                    }
                    break;
            }
        }

        /** Show the previous image */
        private void PreviousImage(Boolean waitForTitleToDisappear)
        {
            if (waitForTitleToDisappear && this.titleShown)
            {
                return;
            }

            switch (this.currentMode)
            {
                case PageMode.Image:
                    if (this.imageIndex > 0)
                    {
                        this.ShowImage(this.imageIndex - 1);
                    }
                    else
                    {
                        this.SetPageMode(PageMode.Start);
                    }
                    break;
                case PageMode.End:
                    this.SetPageMode(PageMode.Image);
                    this.ShowImage(this.imageIndex);
                    break;

            }
        }
    }
}
