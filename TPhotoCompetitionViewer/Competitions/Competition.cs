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
        private string clubName;
        private string trophyName;

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
            this.clubName = rootNode["Club"].InnerText;
            this.trophyName = rootNode["Trophy"].InnerText;
            XmlNode imagesNode = rootNode["Images"];
            foreach (XmlNode eachImageNode in imagesNode.ChildNodes)
            {
                CompetitionImage eachImage = new CompetitionImage(this, eachImageNode);
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

        internal int MaxImageIndex()
        {
            return this.images.Count - 1;
        }

        internal CompetitionImage GetImageObject(int imageIndex)
        {
            return this.images[imageIndex];
        }

        internal object GetClubName()
        {
            return this.clubName;
        }

        internal object GetTrophyName()
        {
            return this.trophyName;
        }

        internal string GetCompetitionDirectory()
        {
            return this.competitionDirectory;
        }
    }
}
