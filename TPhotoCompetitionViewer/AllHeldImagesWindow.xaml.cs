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
            var imageControls = this.BuildArrayOfImageControls();
            var labelControls = this.BuildArrayOfLabelControls();
            this.heldImages = new HeldImages(this.competitionName, heldImages, imageControls, labelControls);

            this.MarkAwardedImages();
        }

        internal void MarkAwardedImages()
        {
            this.heldImages.MarkAwardedImages();
        }

        private List<Label> BuildArrayOfLabelControls()
        {
            List<Label> labelControlList = new List<Label>
            {
                this.Label0,
                this.Label1,
                this.Label2,
                this.Label3,
                this.Label4,
                this.Label5,
                this.Label6,
                this.Label7,
                this.Label8,
                this.Label9,
                this.Label10,
                this.Label11,
                this.Label12,
                this.Label13,
                this.Label14,
                this.Label15,
                this.Label16,
                this.Label17,
                this.Label18,
                this.Label19,
            };

            return labelControlList;
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

        private void ImageClick(int position, object sender, MouseEventArgs e)
        {
            SingleHeldImageWindow heldImageWindow = new SingleHeldImageWindow();

            string databaseFilePath = ImagePaths.GetDatabaseFile(competitionName);
            SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");

            heldImageWindow.Init(this.competitionName, this.heldImages.GetImagePath(position), dbConnection, this);
            heldImageWindow.Show();
        }
    }
}
