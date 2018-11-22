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
        private Competition competition;
        private HeldImages heldImages = null;

        public AllHeldImagesWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleKeys);
        }

        /** Initialise screen with held images */
        internal void Init(Competition competition)
        {
            this.competition = competition;
            var heldImages = CompetitionHelper.GetHeldImages(competition.GetName());
            this.heldImages = new HeldImages(this.competition.GetName(), heldImages, this.gfOuter, this);

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
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.N)
                {
                    this.SelectByNumber();
                }
            }
        }

        private void SelectByNumber()
        {
            ImageNumberPrompt imageNumberWindow = new ImageNumberPrompt();
            imageNumberWindow.ShowDialog();

            int selectedNumber = imageNumberWindow.GetSelectedNumber();
            if (selectedNumber > 0)
            {
                string databaseFilePath = ImagePaths.GetDatabaseFile(this.competition.GetName());
                SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");

                int imagePosition = selectedNumber - 1;
                CompetitionImage image = this.competition.GetImageObject(imagePosition);
                if (image.IsHeld(dbConnection))
                {
                    string imagePath = image.GetFilePath();
                    ShowSingleImageWindow(this.competition.GetName(), imagePath, dbConnection);
                }
            }
        }

        internal void ShowSingleImageWindow(string competitionName, string imagePath, SQLiteConnection dbConnection)
        {
            SingleHeldImageWindow heldImageWindow = new SingleHeldImageWindow();
            heldImageWindow.Init(competitionName, imagePath, dbConnection, this, null);
            heldImageWindow.ShowDialog();
        }
    }
}
