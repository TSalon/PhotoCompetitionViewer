using System;
using System.Collections.Generic;
using System.Data.Common;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using TPhotoCompetitionViewer.Competitions;

namespace TPhotoCompetitionViewer.Views
{
    /// <summary>
    /// Interaction logic for WinnersSlideshowWindow.xaml
    /// </summary>
    public partial class WinnersSlideshowWindow : Window
    {
        private DispatcherTimer titleTimer;
        private string competitionName;
        private List<CompetitionImage> awardedImages;
        private int currentIndex;
        private DispatcherTimer nextImageTimer;

        public WinnersSlideshowWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleKeys);

            //Create a timer with interval of 5 secs for showing the title
            this.titleTimer = new DispatcherTimer();
            this.titleTimer.Tick += new EventHandler(TitleTimer_Tick);
            this.titleTimer.Interval = new TimeSpan(0, 0, 5);

            //Create a timer with interval of 30 secs for showing the next image
            this.nextImageTimer = new DispatcherTimer();
            this.nextImageTimer.Tick += new EventHandler(NextImageTimer_Tick);
            this.nextImageTimer.Interval = new TimeSpan(0, 0, 10);
        }

        public void Init(int competitionIndex)
        {
            Competition competition = CompetitionHelper.GetCompetition(competitionIndex, 0);
            this.competitionName = competition.GetName();
            this.awardedImages = competition.GetAwardedImages();

            this.ShowImage(0);
        }

        /** Show the image at the specified index */
        private void ShowImage(int imageIndex)
        {
            this.titleTimer.Stop();
            this.nextImageTimer.Stop();

            this.currentIndex = imageIndex;
            var awardedImage = this.awardedImages[imageIndex];

            string imagePath = awardedImage.GetFullFilePath();
            BitmapImage imageToShow = new BitmapImage();
            imageToShow.BeginInit();
            imageToShow.UriSource = new Uri(imagePath);
            imageToShow.EndInit();
            this.ImagePane.Source = imageToShow;

            this.ImageTitle.Content = awardedImage.GetTitle();
            this.ImageTitle.Visibility = Visibility.Visible;

            this.ResultsLabel.Content = awardedImage.GetResult() + " - " + awardedImage.GetAuthor();
            this.ResultsLabel.Visibility = Visibility.Visible;

            this.titleTimer.Start();
            this.nextImageTimer.Start();
        }


        /** Handle a key on the keyboard being pushed */
        private void HandleKeys(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.Close();
                    break;
            }
        }

        /** Handle timer tick to hide image title */
        private void TitleTimer_Tick(object sender, EventArgs e)
        {
            this.ImageTitle.Visibility = Visibility.Hidden;
            this.ResultsLabel.Visibility = Visibility.Hidden;
            
            this.titleTimer.IsEnabled = false;
        }

        /** Handle timer tick to hide image title */
        private void NextImageTimer_Tick(object sender, EventArgs e)
        {
            int maxIndex = this.awardedImages.Count;
            int nextIndex = this.currentIndex + 1;
            if (nextIndex >= maxIndex)
            {
                nextIndex = 0;
            }
            this.ShowImage(nextIndex);
        }
    }
}
