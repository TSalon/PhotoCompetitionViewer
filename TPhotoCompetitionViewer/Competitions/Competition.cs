using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace TPhotoCompetitionViewer.Competitions
{
    class Competition
    {
        private const string ORDER_FILENAME = "order.xml";
        private const string COMPETITION_METADATA_FILENAME = "competition.xml";

        private List<CompetitionImage> images;
        private XmlNode competitionName;
        private string competitionTrophy;
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

            // Load competition details
            XmlDocument competitionDocument = new XmlDocument();
            competitionDocument.Load(competitionDirectory + "/" + COMPETITION_METADATA_FILENAME);
            this.competitionName = competitionDocument.SelectSingleNode("/Competition/Name");
            this.competitionTrophy = competitionDocument.SelectSingleNode("/Competition/Trophy").Value;

            // Load image order details
            XmlDocument orderingDocument = new XmlDocument();
            orderingDocument.Load(competitionDirectory + "/" + ORDER_FILENAME);
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

                CompetitionImage eachImage = new CompetitionImage(imageTitle, imageAuthor, imagePath);
                imageList.Add(eachImage);
            }

            this.images = imageList;   
        }

        internal string GetImageName(int imageIndex)
        {
            return this.images[imageIndex].GetTitle();
        }

        internal int MaxImageIndex()
        {
            return this.images.Count - 1;
        }

        internal string GetImagePath(int imageIndex)
        {
            return this.competitionDirectory + "/" + this.images[imageIndex].GetFilePath();
        }
    }
}
