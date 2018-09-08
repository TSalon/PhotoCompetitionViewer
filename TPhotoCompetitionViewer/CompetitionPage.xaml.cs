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
using System.Windows.Shapes;
using System.Windows.Threading;
using TPhotoCompetitionViewer.Competitions;

namespace TPhotoCompetitionViewer
{
    /// <summary>
    /// Interaction logic for CompetitionPage.xaml
    /// </summary>
    public partial class CompetitionPage : Window
    {
        private Competition competition;
        private int imageIndex = 0;
        private DispatcherTimer dispatcherTimer;

        public CompetitionPage()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleKeys);

            //Create a timer with interval of 5 secs
            this.dispatcherTimer = new DispatcherTimer();
            this.dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            this.dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
           this.ImageTitle.Visibility = Visibility.Hidden;

           this.dispatcherTimer.IsEnabled = false;
        }

        private void HandleKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
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

        private void HoldImage(int imageIndex)
        {
            this.competition.HoldImage(imageIndex);
        }

        internal void Init(CompetitionManager competitionMgr, int competitionIndex)
        {
            this.competition = competitionMgr.GetCompetition(competitionIndex);
            this.ShowImage(imageIndex);
        }

        private void ShowImage(int imageIndex)
        {
            this.imageIndex = imageIndex;
            string imagePath = this.competition.GetImagePath(imageIndex);
            BitmapImage imageToShow = new BitmapImage();
            imageToShow.BeginInit();
            imageToShow.UriSource = new Uri("C:" + imagePath);
            imageToShow.EndInit();
            this.ImagePane.Source = imageToShow;

            this.ShowTitle(imageIndex);
          }

        private void ShowTitle(int imageIndex)
        {
            string imageName = this.competition.GetImageName(imageIndex);
            this.ImageTitle.Content = imageName;
            this.ImageTitle.Visibility = Visibility.Visible;

            this.dispatcherTimer.Start();
        }

        private void NextImage()
        {
            if (this.imageIndex < this.competition.MaxImageIndex())
            {
                this.ShowImage(this.imageIndex + 1);
            }
        }

        private void PreviousImage()
        {
            if (this.imageIndex > 0)
            {
                this.ShowImage(this.imageIndex - 1);
            }
        }
    }
}
