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
            List<Image> imageControlList = new List<Image>();

            imageControlList.Add(this.Image0);
            imageControlList.Add(this.Image1);
            imageControlList.Add(this.Image2);
            imageControlList.Add(this.Image3);

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
    }
}
