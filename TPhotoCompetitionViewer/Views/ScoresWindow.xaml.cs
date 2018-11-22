using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TPhotoCompetitionViewer.Competitions;

namespace TPhotoCompetitionViewer.Views
{
    /// <summary>
    /// Interaction logic for ScoresWindow.xaml
    /// </summary>
    public partial class ScoresWindow : Window
    {
        private Competition competition;

        public ScoresWindow()
        {
            InitializeComponent();
        }

        internal void Init(Competition competition)
        {
            this.competition = competition;

            this.ShowHighestScorePerPerson();
        }

        /** Show all scored images on data grid */
        private void ShowAllScoredImages()
        {
            List<CompetitionImage> images = this.competition.GetAllScoredImages();
            this.ShowImages(images);
        }

        public void ShowHighestScorePerPerson()
        {
            List<CompetitionImage> authorTopScoredImages = new List<CompetitionImage>();
            List<CompetitionImage> images = this.competition.GetAllScoredImages();

            foreach (CompetitionImage image in images)
            {
                string thisImageAuthor = image.GetAuthor();
                short thisImageScore = image.GetScore();
                bool alreadyAcceptedAuthor = false;

                // Is there an image already in authorTopScoredImages for this author?
                foreach (CompetitionImage acceptedImage in authorTopScoredImages)
                {
                    if (thisImageAuthor == acceptedImage.GetAuthor())
                    {
                        alreadyAcceptedAuthor = true;
                        // We already have an image from this author
                        if (thisImageScore >= acceptedImage.GetScore())
                        {
                            // The score on the new image is same or better than the one we already have
                            // We should be working top score down, so this likely indicates an image with the same score
                            // Include both
                            authorTopScoredImages.Add(image);
                            break;
                        }
                    }
                }

                if (!alreadyAcceptedAuthor)
                {
                    authorTopScoredImages.Add(image);
                }
            }

            this.ShowImages(authorTopScoredImages);
        }

        private void ShowImages(List<CompetitionImage> images)
        {
            string databaseFilePath = ImagePaths.GetDatabaseFile(this.competition.GetName());
            var dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");

            for (int i = 0; i < images.Count; i++)
            {
                var item = new ScoredImageDataGridItem
                {
                    Author = images[i].GetAuthor(),
                    Title = images[i].GetTitle(),
                    Score = images[i].GetScore(),
                    Timestamp = images[i].GetScoreTimestamp(),
                    ImagePath = images[i].GetFilePath(),
                    Held = images[i].IsHeld(dbConnection),
                };
                this.scoresDataGrid.Items.Add(item);
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            ScoredImageDataGridItem rowData = row.DataContext as ScoredImageDataGridItem;
            string imagePath = rowData.ImagePath;

            SingleHeldImageWindow imageWindow = new SingleHeldImageWindow();
            imageWindow.Init(this.competition.GetName(), imagePath, null, null);
            imageWindow.ShowDialog();
        }
    }
}
