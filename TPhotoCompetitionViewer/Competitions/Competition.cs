using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace TPhotoCompetitionViewer.Competitions
{
    class Competition
    {
        private const string CONTROL_FILENAME = "competition.xml";

        private List<CompetitionImage> images;
        private readonly string competitionFileName;
        private readonly int scoresRequired;
        private string competitionDirectory;

        public Competition(string competitionFileName, int scoresRequired)
        {
            this.competitionFileName = competitionFileName;
            this.scoresRequired = scoresRequired;
        }

        internal void LoadImages(string competitionDirectory)
        {
            // Store path to images
            this.competitionDirectory = competitionDirectory;

            // Load image order details
            XmlDocument orderingDocument = new XmlDocument();
            orderingDocument.Load(competitionDirectory + "/" + CONTROL_FILENAME);
            var imageList = new List<CompetitionImage>();
            XmlNode rootNode = orderingDocument.FirstChild;
            XmlNode imagesNode = rootNode["Images"];
            foreach (XmlNode eachImageNode in imagesNode.ChildNodes)
            {
                string imagePath = eachImageNode.InnerText;
                string[] pathParts = Regex.Split(imagePath, @"/");
                string imageAuthor = pathParts[0];
                string imageFilename = pathParts[1];
                string[] imageFilenameParts = Regex.Split(imageFilename, @"_");
                string memberNumber = imageFilenameParts[0];
                string imageNumber = imageFilenameParts[1];
                string imageTitle = imageFilenameParts[2];
                if (imageFilenameParts.Length > 2)
                {
                    for (int i=3; i<imageFilenameParts.Length; i++)
                    {
                        imageTitle += " " + imageFilenameParts[i];
                    }
                }
                imageTitle = imageTitle.Substring(0, imageTitle.LastIndexOf("."));

                CompetitionImage eachImage = new CompetitionImage(this, imageTitle, imageAuthor, imagePath, imageFilename);
                imageList.Add(eachImage);
            }

            this.images = imageList;   
        }

        internal int GetScoresRequired()
        {
            return this.scoresRequired;
        }

        internal string GetName()
        {
            return this.competitionFileName;
        }

        internal void HoldImage(int imageIndex, SQLiteConnection dbConnection)
        {
            Boolean heldImage = this.images[imageIndex].ToggleHeld(dbConnection);
            if (heldImage)
            {
                this.WriteImageToHeldDirectory(imageIndex);
            }
            else
            {
                this.DeleteImageFromHeldDirectory(imageIndex);
            }
        }

        private void WriteImageToHeldDirectory(int imageIndex)
        {
            String heldDirectory = ImagePaths.GetHeldDirectory(this.competitionFileName);
            if (Directory.Exists(heldDirectory) == false)
            {
                Directory.CreateDirectory(heldDirectory);
            }
            string source = this.GetImagePath(imageIndex);
            string destination = heldDirectory + "/" + this.GetImageFileName(imageIndex) + ".jpg";
            File.Copy(source, destination, true);
        }

        private void DeleteImageFromHeldDirectory(int imageIndex)
        {
            String heldDirectory = ImagePaths.GetHeldDirectory(this.competitionFileName);
            string heldImageFile = heldDirectory + "/" + this.GetImageFileName(imageIndex) + ".jpg";
            File.Delete(heldImageFile);
        }

        private string GetImageFileName(int imageIndex)
        {
            return this.images[imageIndex].GetFilename();
        }

        internal int MaxImageIndex()
        {
            return this.images.Count - 1;
        }

        internal string GetImagePath(int imageIndex)
        {
            return this.competitionDirectory + "/" + this.images[imageIndex].GetFilePath();
        }

        internal CompetitionImage GetImageObject(int imageIndex)
        {
            return this.images[imageIndex];
        }
    }
}
