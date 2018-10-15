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
using TPhotoCompetitionViewer.Competitions;

namespace TPhotoCompetitionViewer
{
    /// <summary>
    /// Interaction logic for HeldWindow.xaml
    /// </summary>
    public partial class AllHeldImagesWindow : Window
    {
        public AllHeldImagesWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleKeys);
        }

        /** Initialise screen with held images */
        internal void Init(CompetitionManager competitionMgr, int competitionIndex)
        {
            Competition competition = competitionMgr.GetCompetition(competitionIndex, 0);
            List<string> heldImagesList = competitionMgr.GetHeldImages(competition.GetName());
            List<Image> imageControls = this.BuildArrayOfImageControls();

            this.AssignImagesToControls(heldImagesList, imageControls, competition.GetName());
        }

        private void AssignImagesToControls(List<string> heldImagesList, List<Image> imageControls, string competitionName)
        {
            string basePath = ImagePaths.GetExtractDirectory(competitionName);
            for (int i=0; i < heldImagesList.Count; i++)
            {
                string imageName = heldImagesList[i];
                string imagePath = basePath + "/" + imageName;
                BitmapImage imageToShow = new BitmapImage();
                imageToShow.BeginInit();
                imageToShow.UriSource = new Uri(imagePath);
                imageToShow.EndInit();

                imageControls[i].Source = imageToShow;
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

        private void ImageClick(int v, object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
