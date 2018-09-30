using System;
using System.Collections.Generic;
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
        private string competitionDirectory;

        public Competition(string competitionFileName)
        {
            this.competitionFileName = competitionFileName;
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
            foreach (XmlNode eachImageNode in rootNode.ChildNodes)
            {
                string imagePath = eachImageNode.InnerText;
                string[] pathParts = Regex.Split(imagePath, @"/");
                string imageAuthor = pathParts[0];
                string imageFilename = pathParts[1];
                string[] imageFilenameParts = Regex.Split(imageFilename, @"_");
                string memberNumber = imageFilenameParts[0];
                string imageNumber = imageFilenameParts[1];
                string imageTitle = imageFilenameParts[2].Substring(0, imageFilenameParts[2].LastIndexOf("."));
                if (imageFilenameParts.Length > 3)
                {
                    for (int i=3; i<imageFilenameParts.Length; i++)
                    {
                        imageTitle += " " + imageFilenameParts[i];
                    }
                }

                CompetitionImage eachImage = new CompetitionImage(imageTitle, imageAuthor, imagePath, imageFilename);
                imageList.Add(eachImage);
            }

            this.images = imageList;   
        }

        internal string GetName()
        {
            return this.competitionFileName;
        }

        internal void HoldImage(int imageIndex)
        {
            Boolean heldImage = this.images[imageIndex].ToggleHeld();
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
            return this.images[imageIndex].getFilename();
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
