using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
                string imageid = image.GetImageId();
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
            string lHash = this.CalculateHash(lAwardString, competition.GetResultsKey());
            string lUrl = lDomain + competition.GetCompetitionKey() + "/" + competition.GetResultsKey() + "/" + lAwardString + "/" + lHash + "/";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(lUrl, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeBitmap = qrCode.GetGraphic(20);

            this.QrCodeImageControl.Source = this.BitmapToImageSource(qrCodeBitmap);
        }

        private string CalculateHash(string pAwardString, string pResultsRandomString)
        {
            string lStringToHash = pAwardString + "_" + pResultsRandomString;
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(lStringToHash));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString().ToLower();
            }
        }
    }
}
