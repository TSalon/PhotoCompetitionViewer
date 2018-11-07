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

            try
            {
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
            catch (Exception e)
            {
                this.clubName = "Competition zip file broken";
                this.trophyName = e.Message;
            }

        }

        /** Return images with results if there are any, or return all the held images if there aren't */
        internal List<CompetitionImage> GetSlideshowImages()
        {
            List<CompetitionImage> awardedImages = new List<CompetitionImage>();

            string databaseFilePath = ImagePaths.GetDatabaseFile(this.GetName());
            SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");
            dbConnection.Open();

            string winners_sql = "SELECT name, position FROM winners";

            SQLiteCommand winners_cmd = new SQLiteCommand(winners_sql, dbConnection);
            SQLiteDataReader winners_reader = winners_cmd.ExecuteReader();
            if (winners_reader.HasRows)
            {
                while (winners_reader.Read())
                {
                    var imageName = winners_reader.GetString(0);
                    var result = winners_reader.GetString(1);

                    foreach (CompetitionImage eachImage in this.images)
                    {
                        if (eachImage.GetFilePath() == imageName)
                        {
                            eachImage.SetResult(result);
                            awardedImages.Add(eachImage);
                            break;
                        }
                    }
                }
            }
            else
            {
                // No images with results, so return all held images instead
                string held_sql = "SELECT name FROM held_images";

                SQLiteCommand held_cmd = new SQLiteCommand(held_sql, dbConnection);
                SQLiteDataReader held_reader = held_cmd.ExecuteReader();
                if (held_reader.HasRows)
                {
                    while (held_reader.Read())
                    {
                        var imageName = held_reader.GetString(0);

                        foreach (CompetitionImage eachImage in this.images)
                        {
                            if (eachImage.GetFilePath() == imageName)
                            {
                                awardedImages.Add(eachImage);
                                break;
                            }
                        }
                    }
                }
            }

            dbConnection.Close();

            return awardedImages;
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
