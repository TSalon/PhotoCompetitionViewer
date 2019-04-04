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

namespace TPhotoCompetitionViewer.Views
{
    /// <summary>
    /// Interaction logic for ResultsUploadWindow.xaml
    /// </summary>
    public partial class ResultsUploadWindow : Window
    {
        public ResultsUploadWindow()
        {
            InitializeComponent();
        }

        internal String BuildAwardString(AbstractCompetition competition)
        {
            string lAwardString = "";

            List<CompetitionImage> lAwardedImages = competition.GetAwardedImages();
            foreach (CompetitionImage image in lAwardedImages)
            {
                string imageid = image.getImageId();
                string position = image.GetAwardCode();

                if (lAwardString.Length > 1)
                {
                    lAwardString += "_";
                }

                lAwardString += imageid;
                lAwardString += "$";
                lAwardString += position;
            }
            return lAwardString;
        }

        internal void Init(AbstractCompetition competition)
        {
            string lAwardString = this.BuildAwardString(competition);
            string lDomain = "http://irisphotosoftware.co.uk/";
            string lUrl = lDomain + competition.GetCompetitionKey() + "/" + competition.GetResultsKey() + "/" + lAwardString + "/";

            this.UrlLabel.Content = lUrl;
        }
    }
}
