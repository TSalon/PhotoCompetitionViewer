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

namespace TPhotoCompetitionViewer
{
    /// <summary>
    /// Interaction logic for CompetitionPage.xaml
    ///   - Responsible for handling all interaction with scoring controllers
    /// </summary>
    public partial class CompetitionPage : Window
    {
        private Competition competition;
        private CompetitionImage competitionImage;
        private int imageIndex = 0;
        private DispatcherTimer dispatcherTimer;
        private HandsetWrapper handsets;
        private SQLiteConnection dbConnection;

        /** Initialise Window */
        public CompetitionPage()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleKeys);

            //Create a timer with interval of 5 secs
            this.dispatcherTimer = new DispatcherTimer();
            this.dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            this.dispatcherTimer.Interval = new TimeSpan(0, 0, 5);

            // get handle to buzz controllers and register event handler
            this.handsets = new HandsetWrapper(new BuzzHandsetFinder().FindHandsets().ToList());
            this.handsets.Get(0).ButtonChanged += HandleHandsetEvent0;
            if (this.handsets.HasSecondHandsetGroup())
            {
                this.handsets.Get(1).ButtonChanged += HandleHandsetEvent1;
            }

            // start with all handset lights turned off
            this.handsets.AllLightsOff();
        }


        /** Handle timer tick to hide image title */
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
           this.ImageTitle.Visibility = Visibility.Hidden;

           this.dispatcherTimer.IsEnabled = false;
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
            if (this.competitionImage != null)
            {
                int totalScore = this.competitionImage.ScoreImage(handsetId, score, this.dbConnection);
                this.handsets.SetLightsForThisImage(this.competitionImage);

                if (totalScore > 0)
                {
                    // we have total score
                    this.Dispatcher.Invoke(() =>
                    {
                        // Wait a bit, so that humans don't think we're reading their mind
                        Thread.Sleep(1500);

                        // Read out the appropriate score
                        this.MediaElement.Source = new Uri("Resources/Numbers/Brian/" + totalScore + ".mp3", UriKind.Relative);
                        this.MediaElement.Play();

                        // allow time for number to be read out and human perception time
                        Thread.Sleep(2500);

                        // Move to next image
                        this.NextImage();
                    });
                }
            }
        }

        /** Handle a key on the keyboard being pushed */
        private void HandleKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.CloseWindow();
            }
            else if (e.Key == Key.Right)
            {
                this.NextImage();
            }
            else if (e.Key == Key.Left)
            {
                this.PreviousImage();
            }
            else if (e.Key == Key.T)
            {
                this.ShowTitle(this.imageIndex);
            }
            else if (e.Key == Key.H)
            {
                this.HoldImage(this.imageIndex);
            }
        }

        private void CloseWindow()
        {
            this.handsets.AllLightsOff();
            this.dbConnection.Close();
            this.Close();
        }

        /** Mark an image as held */
        private void HoldImage(int imageIndex)
        {
            this.competition.HoldImage(imageIndex);
        }

        /** Initialise this window for a particular competition and show the first image */
        internal void Init(CompetitionManager competitionMgr, int competitionIndex, int scoresRequired)
        {
            // Get competition instance
            this.competition = competitionMgr.GetCompetition(competitionIndex, scoresRequired);

            // Get a handle to the database for this competition
            string databaseFilePath = ImagePaths.GetDatabaseFile(this.competition.GetName());
            this.dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");
            this.dbConnection.Open();

            // Show first image
            this.imageIndex = 0;
            this.ShowImage(this.imageIndex);
        }

        /** Show the image at the specified index */
        private void ShowImage(int imageIndex)
        {
            this.imageIndex = imageIndex;
            this.competitionImage = this.competition.GetImageObject(imageIndex);

            this.handsets.SetLightsForThisImage(this.competitionImage);

            string imagePath = this.competition.GetImagePath(imageIndex);
            BitmapImage imageToShow = new BitmapImage();
            imageToShow.BeginInit();
            imageToShow.UriSource = new Uri("C:" + imagePath);
            imageToShow.EndInit();
            this.ImagePane.Source = imageToShow;

            this.ShowTitle(imageIndex);
        }

        /** Show the image title for a short period of time */
        private void ShowTitle(int imageIndex)
        {
            string imageName = this.competitionImage.GetTitle();

            this.ImageTitle.Content = imageName;
            this.ImageTitle.Visibility = Visibility.Visible;

            this.dispatcherTimer.Start();

        }

        /** Show the next image */
        private void NextImage()
        {
            if (this.imageIndex < this.competition.MaxImageIndex())
            {
                this.ShowImage(this.imageIndex + 1);
            }
            else
            {
                this.ImagePane.Source = null;
                this.competitionImage = null;
                this.ImageTitle.Content = "Finished";
            }
        }

        /** Show the previous image */
        private void PreviousImage()
        {
            if (this.imageIndex > 0)
            {
                this.ShowImage(this.imageIndex - 1);
            }
        }
    }
}
