using System;
using System.Collections.Generic;
using System.Data.Common;
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

        /** Show specified image in window */
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
            string resultPosition = "";
            switch (result)
            {
                case Result.None:
                    resultPosition = null;
                    break;
                case Result.First:
                    resultPosition = "First Place";
                    break;
                case Result.Second:
                    resultPosition = "Second Place";
                    break;
                case Result.Third:
                    resultPosition = "Third Place";
                    break;
                case Result.HighlyCommended:
                    resultPosition = "Highly Commended";
                    break;
                case Result.Commended:
                    resultPosition = "Commended";
                    break;
            }

            if (resultPosition != null)
            {
                this.ImagePosition.Content = resultPosition;
                this.ImagePosition.Visibility = Visibility.Visible;

                this.ImageAuthor.Content = this.imageAuthor;
                this.ImageAuthor.Visibility = Visibility.Visible;
            }
            else
            {
                this.ImagePosition.Content = "Cleared";
                this.ImagePosition.Visibility = Visibility.Visible;
            }
            this.WriteResultToDatabase(this.imageFileName, resultPosition);

            this.allHeldImagesWindow.MarkAwardedImages();
        }

        private void WriteResultToDatabase(string imageFileName, string resultPosition)
        {
            this.dbConnection.Open();

            if (resultPosition != null)
            {
                String sql = "INSERT INTO winners (timestamp, name, position) VALUES (CURRENT_TIMESTAMP, @name, @position)";

                SQLiteCommand insertWinner = new SQLiteCommand(sql, this.dbConnection);
                insertWinner.Parameters.Add(new SQLiteParameter("@name", imageFileName));
                insertWinner.Parameters.Add(new SQLiteParameter("@position", resultPosition));
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
