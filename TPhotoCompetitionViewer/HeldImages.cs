using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TPhotoCompetitionViewer.Competitions;

namespace TPhotoCompetitionViewer
{
    class HeldImages
    {
        private const int IMAGES_ACROSS = 5;

        private readonly List<Image> imageControls = new List<Image>();
        private readonly List<Label> labelControls = new List<Label>();
        private readonly string competitionName;
        private List<string> imageFilePaths;
        private Dictionary<string, string> awards;
        private readonly Grid gfOuter;
        private readonly AllHeldImagesWindow AllHeldImagesWindow;

        public static readonly DependencyProperty ImageNumberProperty = DependencyProperty.RegisterAttached("ImageNumber", typeof(int), typeof(Image), new PropertyMetadata(default(int)));


        public HeldImages(string competitionName, List<string> heldImages, Grid gfOuter, AllHeldImagesWindow allHeldImagesWindow)
        {
            this.competitionName = competitionName;
            this.imageFilePaths = heldImages;
            this.gfOuter = gfOuter;
            this.AllHeldImagesWindow = allHeldImagesWindow;

            this.BuildArrayOfControls(heldImages.Count);
            
            this.AssignImagesToControls();
        }

        private void BuildArrayOfControls(int imageCount)
        {
            int lRows = (imageCount / IMAGES_ACROSS) + 1;
            for (int i=0; i<lRows; i++)
            {
                this.gfOuter.RowDefinitions.Add(new RowDefinition());
            }

            int gridRow = 0;
            int gridColumn = 0;

            for (int i=0; i<imageCount; i++)
            {
                Image eachImage = new Image
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    MaxWidth = 250,
                    MaxHeight = 200,
                };
                eachImage.SetValue(ImageNumberProperty, i);
                eachImage.MouseDown += EachImage_MouseDown;
                this.imageControls.Add(eachImage);

                Border eachBorder = new Border
                {
                    Child = eachImage,
                };
                Grid.SetRow(eachBorder, gridRow);
                Grid.SetColumn(eachBorder, gridColumn);
                this.gfOuter.Children.Add(eachBorder);



                Label eachLabel = new Label
                {
                    Content = "",
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(0),
                    Visibility = Visibility.Hidden,
                    Opacity = 0.5,
                    FontSize = 72,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Background = new SolidColorBrush(Colors.Black),
                    IsHitTestVisible = false,
                };
                eachLabel.Background.Opacity = 0.7;
                this.labelControls.Add(eachLabel);
                Grid.SetRow(eachLabel, gridRow);
                Grid.SetColumn(eachLabel, gridColumn);
                this.gfOuter.Children.Add(eachLabel);

                gridColumn++;
                if (gridColumn > (IMAGES_ACROSS - 1)){
                    gridColumn = 0;
                    gridRow++;
                }
            }
        }

        private void EachImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image clickedImage = (Image)sender;
            int position = (int)clickedImage.GetValue(ImageNumberProperty);
        
            string databaseFilePath = ImagePaths.GetDatabaseFile(competitionName);
            SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");

            this.AllHeldImagesWindow.ShowSingleImageWindow(this.competitionName, this.GetImagePath(position), dbConnection);
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

            string sql = "SELECT name, position, CASE WHEN position = '1' THEN 1 WHEN position = '2' THEN 2 WHEN position = '3' THEN 3 WHEN position = 'HC' THEN 4 WHEN position = 'C' THEN 5 ELSE 0 END FROM winners order by 3";

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

            // Take the existing image paths, and remove those with awards
            this.imageFilePaths = this.imageFilePaths.Except(this.awards.Keys).ToList();
            // Add the award holders on at the end
            this.imageFilePaths.AddRange(this.awards.Keys);
            this.AssignImagesToControls();

            // Grey out images with positions
            for (int i = 0; i < this.imageFilePaths.Count(); i++)
            {
                foreach (KeyValuePair<string, string> entry in this.awards)
                {
                    if (this.imageFilePaths[i] == entry.Key)
                    {
                        this.imageControls[i].Opacity = 0.2;
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
