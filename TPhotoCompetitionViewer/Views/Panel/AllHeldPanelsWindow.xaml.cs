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

namespace TPhotoCompetitionViewer.Views.Panel
{
    /// <summary>
    /// Interaction logic for HeldWindow.xaml
    /// </summary>
    public partial class AllHeldPanelsWindow : Window, IAllHeldImagesWindow
    {
        private PanelCompetition competition;
        private HeldImages heldImages = null;

        public AllHeldPanelsWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleKeys);
        }

        /** Initialise screen with held images */
        internal void Init(PanelCompetition competition)
        {
            this.competition = competition;
            var heldImages = CompetitionHelper.GetHeldImages(competition.GetName());
            this.heldImages = new HeldImages(this.competition.GetName(), heldImages, this.gfOuter, this);

            this.MarkAwardedImages();
        }

        public void MarkAwardedImages()
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

        public void ShowSingleImageWindow(string competitionName, string imagePath, SQLiteConnection dbConnection)
        {
            SingleHeldImageWindow heldImageWindow = new SingleHeldImageWindow();
            heldImageWindow.Init(competitionName, imagePath, dbConnection, this, null);
            heldImageWindow.ShowDialog();
        }
    }
}
