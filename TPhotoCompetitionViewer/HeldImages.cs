using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TPhotoCompetitionViewer.Competitions;

namespace TPhotoCompetitionViewer
{
    class HeldImages
    {
        private readonly List<Image> imageControls;
        private readonly List<Label> labelControls;
        private readonly string competitionName;
        private readonly List<string> imageFilePaths;
        private Dictionary<string, string> awards;

        public HeldImages(string competitionName, List<String> imageList, List<Image> controlList, List<Label> labelControlsList)
        {
            this.competitionName = competitionName;
            this.imageFilePaths = imageList;
            this.imageControls = controlList;
            this.labelControls = labelControlsList;

            this.AssignImagesToControls();
        }
               
        private void AssignImagesToControls()
        {
            string basePath = ImagePaths.GetExtractDirectory(this.competitionName);
            for (int i = 0; i < this.imageFilePaths.Count; i++)
            {
                string imageName = this.imageFilePaths[i];
                string imagePath = basePath + "/" + imageName;
                BitmapImage imageToShow = new BitmapImage();
                imageToShow.BeginInit();
                imageToShow.UriSource = new Uri(imagePath);
                imageToShow.EndInit();

                this.imageControls[i].Source = imageToShow;
            }
        }

        internal void MarkAwardedImages()
        {
            // Fetch list of awarded images
            this.awards = new Dictionary<string, string>();
            string databaseFilePath = ImagePaths.GetDatabaseFile(this.competitionName);
            SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");
            dbConnection.Open();

            string sql = "SELECT name, position FROM winners";

            SQLiteCommand cmd = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string imageName = reader.GetString(0);
                    string imageAward = reader.GetString(1);
                    this.awards[imageName] = imageAward;
                }
            }

            dbConnection.Close();

            // Make all visible
            foreach (var eachImageControl in this.imageControls)
            {
                eachImageControl.Opacity = 1.0;
            }
            foreach (var eachLabelControl in this.labelControls)
            {
                eachLabelControl.Visibility = Visibility.Hidden;
            }

            // Grey out images with positions
            for (int i = 0; i < this.imageFilePaths.Count(); i++)
            {
                foreach (KeyValuePair<string, string> entry in this.awards)
                {
                    if (this.imageFilePaths[i] == entry.Key)
                    {
                        this.imageControls[i].Opacity = 0.3;
                        this.labelControls[i].Content = entry.Value;
                        this.labelControls[i].Visibility = Visibility.Visible;
                        continue;
                    }
                }
            }
        }

        internal string GetImagePath(int position)
        {
            return this.imageFilePaths[position];
        }
    }


}
