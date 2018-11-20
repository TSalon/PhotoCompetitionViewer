using System;
using System.Collections.Generic;
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

            this.ShowAllScoredImages();
        }

        /** Show all scored images on data grid */
        private void ShowAllScoredImages()
        {
            List<CompetitionImage> images = this.competition.GetAllScoredImages();
            this.ShowImages(images);
        }

        private void ShowImages(List<CompetitionImage> images)
        { 
            for (int i = 0; i < images.Count; i++)
            {
                var item = new ScoredImageDataGridItem
                {
                    Author = images[i].GetAuthor(),
                    Title = images[i].GetTitle(),
                    Score = images[i].GetScore(),
                    Timestamp = images[i].GetScoreTimestamp(),
                    ImagePath = images[i].GetFilePath(),
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
