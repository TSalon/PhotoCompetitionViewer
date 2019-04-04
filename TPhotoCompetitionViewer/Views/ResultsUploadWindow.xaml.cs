using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

        internal BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        internal void Init(AbstractCompetition competition)
        {
            string lAwardString = this.BuildAwardString(competition);
            string lDomain = "http://irisphotosoftware.co.uk/";
            string lUrl = lDomain + competition.GetCompetitionKey() + "/" + competition.GetResultsKey() + "/" + lAwardString + "/";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(lUrl, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeBitmap = qrCode.GetGraphic(20);

            this.QrCodeImageControl.Source = this.BitmapToImageSource(qrCodeBitmap);
        }
    }
}
