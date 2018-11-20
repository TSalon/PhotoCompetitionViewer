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
        internal List<CompetitionImage> images;

        public ScoresWindow()
        {
            InitializeComponent();
        }

        internal void Init(Competition competition)
        {
            this.competition = competition;
            this.images = this.competition.GetScoredImages();

            this.PopulateDataGrid();
        }

        private void PopulateDataGrid()
        {
            for (int i = 0; i < this.images.Count; i++)
            {
                this.scoresDataGrid.Items.Add(new { Author = this.images[i].GetAuthor(),
                                                    Title = this.images[i].GetTitle(),
                                                    Score = this.images[i].GetScore(),
                                                    Timestamp = this.images[i].GetScoreTimestamp(),
                                                  });
            }
        }
    }
}
