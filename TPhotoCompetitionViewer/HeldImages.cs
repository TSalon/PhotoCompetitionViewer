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
using TPhotoCompetitionViewer.Views;

namespace TPhotoCompetitionViewer
{
    class HeldImages
    {
        private const int IMAGES_ACROSS = 5;

        private readonly List<Image> imageControls = new List<Image>();
        private readonly List<Label> labelControls = new List<Label>();
        private readonly List<Label> titleControls = new List<Label>();
        private readonly AbstractCompetition competition;
        private List<CompetitionImage> images;
        private Dictionary<string, string> awards;
        private readonly Grid gfOuter;
        private readonly IAllHeldImagesWindow AllHeldImagesWindow;

        public static readonly DependencyProperty ImageNumberProperty = DependencyProperty.RegisterAttached("ImageNumber", typeof(int), typeof(Image), new PropertyMetadata(default(int)));


        public HeldImages(AbstractCompetition competition, List<CompetitionImage> heldImages, Grid gfOuter, IAllHeldImagesWindow allHeldImagesWindow)
        {
            this.competition = competition;
            this.images = heldImages;
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
                };
                if (lRows < 3)
                {
                    eachImage.MaxWidth = 500;
                    eachImage.MaxHeight = 400;
                }
                else
                {
                    eachImage.MaxWidth = 250;
                    eachImage.MaxHeight = 200;
                }
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

                Label eachTitle = new Label()
                {
                    Content = "",
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(0),
                    Visibility = Visibility.Visible,
                    Opacity = 0.5,
                    FontSize = 16,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Background = new SolidColorBrush(Colors.Black),
                    IsHitTestVisible = false,
                };
                eachTitle.Background.Opacity = 0.7;
                this.titleControls.Add(eachTitle);
                Grid.SetRow(eachTitle, gridRow);
                Grid.SetColumn(eachTitle, gridColumn);
                this.gfOuter.Children.Add(eachTitle);

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
        
            string databaseFilePath = ImagePaths.GetDatabaseFile(this.competition.GetName());
            SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");

            this.AllHeldImagesWindow.ShowSingleImageWindow(this.competition.GetName(), this.GetImagePath(position), dbConnection);
        }

        private void AssignImagesToControls()
        {
            string basePath = ImagePaths.GetExtractDirectory(this.competition.GetName());
            for (int i = 0; i < this.images.Count; i++)
            {
                string imageName = this.images[i].GetFilePath();
                string imagePath = basePath + "/" + imageName;
                BitmapImage imageToShow = new BitmapImage();
                imageToShow.BeginInit();
                imageToShow.UriSource = new Uri(imagePath);
                imageToShow.EndInit();

                this.imageControls[i].Source = imageToShow;
                this.titleControls[i].Content = this.GetImageTitle(i);
            }
        }

        internal void MarkAwardedImages()
        {
            // Fetch list of awarded images
            this.awards = new Dictionary<string, string>();
            string databaseFilePath = ImagePaths.GetDatabaseFile(this.competition.GetName());
            SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");
            dbConnection.Open();

            string sql = "SELECT w.name, w.position, CASE WHEN w.position = '1' THEN 1 WHEN w.position = '2' THEN 2 WHEN w.position = '3' THEN 3 WHEN w.position = 'HC' THEN 4 WHEN w.position = 'C' THEN 5 ELSE 0 END FROM winners w INNER JOIN held_images h ON (h.name=w.name) order by 3";

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
            foreach (var eachTitleControl in this.titleControls)
            {
                eachTitleControl.Visibility = Visibility.Visible;
            }

            // Take the existing image paths, and remove those with awards
            List<CompetitionImage> awardedImages = new List<CompetitionImage>();
            foreach (string key in this.awards.Keys)
            {
                if (this.competition is PanelCompetition)
                {
                    CompetitionPanel awardedPanel = this.competition.GetImagePanelById(key);
                    CompetitionImage competitionImage = awardedPanel.GetPanelImage();
                    awardedImages.Add(competitionImage);
                }
                else
                {
                    CompetitionImage awardedImage = this.competition.GetImageObjectById(key);
                    awardedImages.Add(awardedImage);
                }
            }
            this.images = this.images.Except(awardedImages).ToList();
            // Add the award holders on at the end
            this.images.AddRange(awardedImages);
            this.AssignImagesToControls();

            // Grey out images with positions
            for (int i = 0; i < this.images.Count(); i++)
            {
                foreach (KeyValuePair<string, string> entry in this.awards)
                {
                    if (this.images[i].GetFilePath() == entry.Key)
                    {
                        this.imageControls[i].Opacity = 0.5;
                        this.labelControls[i].Content = entry.Value;
                        this.labelControls[i].Visibility = Visibility.Visible;
                        this.titleControls[i].Visibility = Visibility.Hidden;
                        continue;
                    }
                }
            }
        }

        internal CompetitionImage GetImagePath(int position)
        {
            return this.images[position];
        }

        internal string GetImageTitle(int position)
        {
            return this.GetImagePath(position).GetTitle();
        }
    }


}
