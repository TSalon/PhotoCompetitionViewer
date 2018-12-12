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

namespace TPhotoCompetitionViewer.Views.Panel
{
    /// <summary>
    /// Interaction logic for PanelCompetitionImageWindow.xaml
    ///   - Scoring controllers not supported
    /// </summary>
    public partial class PanelCompetitionImageWindow : Window
    {
        private enum PageMode { Start, Panel, Image, End };
        private PanelCompetition competition;
        private CompetitionImage competitionImage;
        private CompetitionPanel competitionPanel;
        private int imageIndex = 0;
        private int panelIndex = 0;
        private DispatcherTimer titleTimer;
        private MainWindow mainWindow;
        private SQLiteConnection dbConnection;
        private bool titleShown = false;
        private PageMode currentMode = PageMode.Start;

        /** Initialise Window */
        public PanelCompetitionImageWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleKeys);

            //Create a timer with interval of 5 secs for showing the title
            this.titleTimer = new DispatcherTimer();
            this.titleTimer.Tick += new EventHandler(TitleTimer_Tick);
            this.titleTimer.Interval = new TimeSpan(0, 0, TimingValues.TIME_TO_SHOW_TITLES_FOR_SECONDS);
        }


        /** Initialise this window for a particular competition and show the title page */
        internal void Init(PanelCompetition competition, MainWindow mainWindow)
        {
            // Get handle to main window
            this.mainWindow = mainWindow;

            // Get handle to competition
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
            this.PanelPosition.Visibility = Visibility.Hidden;
            this.ClubNameLabel.Visibility = Visibility.Hidden;
            this.CompetitionNameLabel.Visibility = Visibility.Hidden;
            this.TrophyNameLabel.Visibility = Visibility.Hidden;

            this.titleShown = false;

            this.titleTimer.Stop();
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

                case PageMode.Panel:
                    this.HideAllControls();

                    this.ImagePane.Visibility = Visibility.Visible;
                    this.PanelPosition.Visibility = Visibility.Visible;

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
            switch (this.currentMode)
            {
                case PageMode.Image:
                    this.ImageTitle.Visibility = Visibility.Hidden;
                    this.titleShown = false;
                    this.titleTimer.IsEnabled = false;
                    break;
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
                    this.HoldPanel();
                    break;
            }
        }

        private void CloseWindow()
        {
            this.Close();
        }

        /** Mark an image as held */
        private void HoldPanel()
        {
            if (this.currentMode != PageMode.Panel)
            {
                return;
            }
            
            this.competitionPanel.ToggleHeld(this.dbConnection);
            this.ShowProgress();

            this.mainWindow.UpdateHeldCount();
        }

        /** Show the image at the specified index */
        private void ShowImage(int imageIndex)
        {
	        this.titleTimer.Stop();
	
            this.imageIndex = imageIndex;
            this.competitionImage = this.competition.GetImageObject(this.panelIndex, imageIndex);

            string imagePath = competitionImage.GetFullFilePath();
            BitmapImage imageToShow = new BitmapImage();
            imageToShow.BeginInit();
            imageToShow.UriSource = new Uri(imagePath);
            imageToShow.EndInit();
            this.ImagePane.Source = imageToShow;

            this.ShowTitle();

            this.SetPageMode(PageMode.Image);
        }

        /** Show the image title for a short period of time */
        private void ShowTitle()
        {
            if (this.currentMode != PageMode.Image)
            {
                return;
            }

            string imageName = this.competitionImage.GetTitle();
            this.ImageTitle.Content = imageName;
            this.ImageTitle.Visibility = Visibility.Visible;
           
            this.titleShown = true;

            this.titleTimer.Start();
        }

        private void ShowProgress()
        {
            if (this.currentMode != PageMode.Panel)
            {
                return;
            }

            int panelCount = this.competition.MaxPanelIndex() + 1;
            int panelCurrentIndex = this.panelIndex + 1;
            string panelPosition;

            if (this.competitionPanel.IsHeld(this.dbConnection))
            {
                panelPosition = "[" + panelCurrentIndex + "/" + panelCount + "]";
            }
            else
            {
                panelPosition = "(" + panelCurrentIndex + "/" + panelCount + ")";
            }

            this.PanelPosition.Content = panelPosition;
            this.PanelPosition.Visibility = Visibility.Visible;

            this.titleShown = true;

            this.titleTimer.Start();
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
                    this.SetPageMode(PageMode.Panel);
                    // Show first panel title
                    this.panelIndex = 0;
                    this.ShowPanel(this.panelIndex);
                    break;

                case PageMode.Panel:
                    this.ShowImage(0);

                    break;

                case PageMode.Image:
                    // Show next image, or end page
                    if (this.imageIndex < this.competition.MaxImageIndex(this.panelIndex))
                    {
                        this.ShowImage(this.imageIndex + 1);
                    }
                    else
                    {
                        if (this.panelIndex < this.competition.MaxPanelIndex())
                        {
                            this.ShowPanel(this.panelIndex + 1);
                        }
                        else
                        {
                            this.SetPageMode(PageMode.End);
                        }
                    }
                    break;
            }
        }

        private void ShowPanel(int panelIndex)
        {
            this.SetPageMode(PageMode.Panel);

            this.panelIndex = panelIndex;
            this.competitionPanel = this.competition.GetPanel(panelIndex);
            this.competitionImage = null;

            string imagePath = this.competition.GetCompetitionDirectory() + "/" + this.competitionPanel.GetPanelId();
            BitmapImage imageToShow = new BitmapImage();
            imageToShow.BeginInit();
            imageToShow.UriSource = new Uri(imagePath);
            imageToShow.EndInit();
            this.ImagePane.Source = imageToShow;

            this.ShowProgress();
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
                case PageMode.Panel:
                    if (this.panelIndex == 0)
                    {
                        this.SetPageMode(PageMode.Start);
                    }
                    else
                    {
                        this.panelIndex--;
                        this.SetPageMode(PageMode.Image);
                        CompetitionPanel panel = this.competition.GetPanel(this.panelIndex);
                        this.ShowImage(panel.MaxImageIndex());
                    }
                    break;

                case PageMode.Image:
                    if (this.imageIndex > 0)
                    {
                        this.ShowImage(this.imageIndex - 1);
                    }
                    else
                    {
                        this.ShowPanel(this.panelIndex);
                    }
                    break;

                case PageMode.End:
                    this.ShowImage(this.imageIndex);
                    break;

            }
        }
    }
}
