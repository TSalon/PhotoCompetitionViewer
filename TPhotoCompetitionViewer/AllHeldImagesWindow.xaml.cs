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

namespace TPhotoCompetitionViewer
{
    /// <summary>
    /// Interaction logic for HeldWindow.xaml
    /// </summary>
    public partial class AllHeldImagesWindow : Window
    {
        private string competitionName;
        private List<string> heldImagesList = null;
        private List<Image> imageControls = null;


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
            this.heldImagesList = competitionMgr.GetHeldImages(competition.GetName());
            this.imageControls = this.BuildArrayOfImageControls();

            this.AssignImagesToControls();
            this.GreyOutAwardedImages();
        }

        internal void GreyOutAwardedImages()
        {
            // Fetch list of awarded images
            List<string> awardedImages = new List<string>();
            string databaseFilePath = ImagePaths.GetDatabaseFile(this.competitionName);
            SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");
            dbConnection.Open();

            string sql = "SELECT name FROM winners";

            SQLiteCommand cmd = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    awardedImages.Add(reader.GetString(0));
                }
            }

            dbConnection.Close();

            // Make all visible
            foreach (var eachImageControl in this.imageControls)
            {
                eachImageControl.Opacity = 1.0;
            }

            // Grey out images with positions
            for (int i=0; i<this.heldImagesList.Count();i++)
            {
                foreach (string awardedImage in awardedImages)
                {
                    if (this.heldImagesList[i] == awardedImage)
                    {
                        this.imageControls[i].Opacity = 0.3;
                        continue;
                    }
                }
            }
        }

        private void AssignImagesToControls()
        {
            string basePath = ImagePaths.GetExtractDirectory(this.competitionName);
            for (int i=0; i < this.heldImagesList.Count; i++)
            {
                string imageName = this.heldImagesList[i];
                string imagePath = basePath + "/" + imageName;
                BitmapImage imageToShow = new BitmapImage();
                imageToShow.BeginInit();
                imageToShow.UriSource = new Uri(imagePath);
                imageToShow.EndInit();

                this.imageControls[i].Source = imageToShow;
            }
        }

        private List<Image> BuildArrayOfImageControls()
        {
            List<Image> imageControlList = new List<Image>
            {
                this.Image0,
                this.Image1,
                this.Image2,
                this.Image3,
                this.Image4,
                this.Image5,
                this.Image6,
                this.Image7,
                this.Image8,
                this.Image9,
                this.Image10,
                this.Image11,
                this.Image12,
                this.Image13,
                this.Image14,
                this.Image15,
                this.Image16,
                this.Image17,
                this.Image18,
                this.Image19,
            };

            return imageControlList;
        }

        /** Handle a key on the keyboard being pushed */
        private void HandleKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Image0_Click(object sender, MouseEventArgs e) => this.ImageClick(0, sender, e);
        private void Image1_Click(object sender, MouseEventArgs e) => this.ImageClick(1, sender, e);
        private void Image2_Click(object sender, MouseEventArgs e) => this.ImageClick(2, sender, e);
        private void Image3_Click(object sender, MouseEventArgs e) => this.ImageClick(3, sender, e);
        private void Image4_Click(object sender, MouseEventArgs e) => this.ImageClick(4, sender, e);
        private void Image5_Click(object sender, MouseEventArgs e) => this.ImageClick(5, sender, e);
        private void Image6_Click(object sender, MouseEventArgs e) => this.ImageClick(6, sender, e);
        private void Image7_Click(object sender, MouseEventArgs e) => this.ImageClick(7, sender, e);
        private void Image8_Click(object sender, MouseEventArgs e) => this.ImageClick(8, sender, e);
        private void Image9_Click(object sender, MouseEventArgs e) => this.ImageClick(9, sender, e);
        private void Image10_Click(object sender, MouseEventArgs e) => this.ImageClick(10, sender, e);
        private void Image11_Click(object sender, MouseEventArgs e) => this.ImageClick(11, sender, e);
        private void Image12_Click(object sender, MouseEventArgs e) => this.ImageClick(12, sender, e);
        private void Image13_Click(object sender, MouseEventArgs e) => this.ImageClick(13, sender, e);
        private void Image14_Click(object sender, MouseEventArgs e) => this.ImageClick(14, sender, e);
        private void Image15_Click(object sender, MouseEventArgs e) => this.ImageClick(15, sender, e);
        private void Image16_Click(object sender, MouseEventArgs e) => this.ImageClick(16, sender, e);
        private void Image17_Click(object sender, MouseEventArgs e) => this.ImageClick(17, sender, e);
        private void Image18_Click(object sender, MouseEventArgs e) => this.ImageClick(18, sender, e);
        private void Image19_Click(object sender, MouseEventArgs e) => this.ImageClick(19, sender, e);

        private void ImageClick(int v, object sender, MouseEventArgs e)
        {
            SingleHeldImageWindow heldImageWindow = new SingleHeldImageWindow();

            string databaseFilePath = ImagePaths.GetDatabaseFile(competitionName);
            SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");

            heldImageWindow.Init(this.competitionName, this.heldImagesList[v], dbConnection, this);
            heldImageWindow.Show();
        }
    }
}
