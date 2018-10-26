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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TPhotoCompetitionViewer.Competitions;

namespace TPhotoCompetitionViewer.Views
{
    /// <summary>
    /// Interaction logic for HeldWindow.xaml
    /// </summary>
    public partial class AllHeldImagesWindow : Window
    {
        private string competitionName;
        private HeldImages heldImages = null;

        public AllHeldImagesWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleKeys);
        }

        /** Initialise screen with held images */
        internal void Init(CompetitionManager competitionMgr, int competitionIndex)
        {
            Competition competition = competitionMgr.GetCompetition(competitionIndex, 0);
            this.competitionName = competition.GetName();
            var heldImages = competitionMgr.GetHeldImages(competition.GetName());
            this.heldImages = new HeldImages(this.competitionName, heldImages, this.gfOuter, this);

            this.MarkAwardedImages();
        }

        internal void MarkAwardedImages()
        {
            this.heldImages.MarkAwardedImages();
        }

        /** Handle a key on the keyboard being pushed */
        private void HandleKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        internal void ShowSingleImageWindow(string competitionName, string imagePosition, SQLiteConnection dbConnection)
        {
            SingleHeldImageWindow heldImageWindow = new SingleHeldImageWindow();
            heldImageWindow.Init(competitionName, imagePosition, dbConnection, this);
            heldImageWindow.ShowDialog();
        }
    }
}
