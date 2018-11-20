using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for SingleHeldImageWindow.xaml
    /// </summary>
    public partial class SingleHeldImageWindow : Window
    {
        private SQLiteConnection dbConnection;
        private string imageFileName;
        private AllHeldImagesWindow allHeldImagesWindow;
        private string imageAuthor;

        enum Result { First, Second, Third, HighlyCommended, Commended, None };

        public SingleHeldImageWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleKeys);
        }

        /** Show specified image in window.
         *  Use null allHeldImagesWindow to use this standalone, with no scoring.
         */
        internal void Init(string competitionName, string imageName, SQLiteConnection dbConnection, AllHeldImagesWindow allHeldImagesWindow)
        {
            this.dbConnection = dbConnection;
            this.imageFileName = imageName;
            this.allHeldImagesWindow = allHeldImagesWindow;

            this.imageAuthor = imageName.Split('/')[0];

            string basePath = ImagePaths.GetExtractDirectory(competitionName);
            string imagePath = basePath + "/" + imageName;
            BitmapImage imageToShow = new BitmapImage();
            imageToShow.BeginInit();
            imageToShow.UriSource = new Uri(imagePath);
            imageToShow.EndInit();

            this.ImagePane.Source = imageToShow;

            this.ImagePosition.Visibility = Visibility.Hidden;
            this.ImageAuthor.Visibility = Visibility.Hidden;
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
                if (e.Key == Key.D1)
                {
                    this.AwardResult(Result.First);
                }
                else if (e.Key == Key.D2)
                {
                    this.AwardResult(Result.Second);
                }
                else if (e.Key == Key.D3)
                {
                    this.AwardResult(Result.Third);
                }
                else if (e.Key == Key.D0)
                {
                    this.AwardResult(Result.None);
                }
                else if (e.Key == Key.C)
                {
                    this.AwardResult(Result.Commended);
                }
                else if (e.Key == Key.H)
                {
                    this.AwardResult(Result.HighlyCommended);
                }
            }
        }

        private void AwardResult(Result result)
        {
            if (this.allHeldImagesWindow != null) // don't do anything if we're no in the context of held images
            {
                string resultPosition = "";
                string shortPosition = "";
                switch (result)
                {
                    case Result.None:
                        resultPosition = null;
                        shortPosition = null;
                        break;
                    case Result.First:
                        resultPosition = "First Place";
                        shortPosition = "1";
                        break;
                    case Result.Second:
                        resultPosition = "Second Place";
                        shortPosition = "2";
                        break;
                    case Result.Third:
                        resultPosition = "Third Place";
                        shortPosition = "3";
                        break;
                    case Result.HighlyCommended:
                        resultPosition = "Highly Commended";
                        shortPosition = "HC";
                        break;
                    case Result.Commended:
                        resultPosition = "Commended";
                        shortPosition = "C";
                        break;
                }

                if (resultPosition != null)
                {
                    this.ImagePosition.Content = resultPosition;
                    this.ImagePosition.Visibility = Visibility.Visible;

                    CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                    TextInfo textInfo = cultureInfo.TextInfo;

                    this.ImageAuthor.Content = textInfo.ToTitleCase(this.imageAuthor);
                    this.ImageAuthor.Visibility = Visibility.Visible;
                }
                else
                {
                    this.ImagePosition.Content = "Cleared";
                    this.ImagePosition.Visibility = Visibility.Visible;
                }

                this.WriteResultToDatabase(this.imageFileName, shortPosition);

                this.allHeldImagesWindow.MarkAwardedImages();
            }
        }

        private void WriteResultToDatabase(string imageFileName, string shortPosition)
        {
            this.dbConnection.Open();

            if (shortPosition != null)
            {
                String sql = "INSERT INTO winners (timestamp, name, position) VALUES (CURRENT_TIMESTAMP, @name, @position)";

                SQLiteCommand insertWinner = new SQLiteCommand(sql, this.dbConnection);
                insertWinner.Parameters.Add(new SQLiteParameter("@name", imageFileName));
                insertWinner.Parameters.Add(new SQLiteParameter("@position", shortPosition));
                insertWinner.ExecuteNonQuery();
            }
            else
            {
                String sql = "DELETE FROM winners WHERE name = @name";

                SQLiteCommand deleteAward = new SQLiteCommand(sql, this.dbConnection);
                deleteAward.Parameters.Add(new SQLiteParameter("@name", imageFileName));
                deleteAward.ExecuteNonQuery();
            }
            this.dbConnection.Close();
        }
    }
}
