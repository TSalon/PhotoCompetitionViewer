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
    /// Interaction logic for SingleHeldImageWindow.xaml
    /// </summary>
    public partial class SingleHeldImageWindow : Window
    {
        public SingleHeldImageWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleKeys);
        }

        /** Show specified image in window */
        internal void Init(string competitionName, string imageName)
        {
            string basePath = ImagePaths.GetExtractDirectory(competitionName);
           
            string imagePath = basePath + "/" + imageName;
            BitmapImage imageToShow = new BitmapImage();
            imageToShow.BeginInit();
            imageToShow.UriSource = new Uri(imagePath);
            imageToShow.EndInit();

            this.ImagePane.Source = imageToShow;
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
