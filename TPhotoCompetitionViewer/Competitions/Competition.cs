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
    class Competition : AbstractCompetition
    {
        private List<CompetitionImage> images = new List<CompetitionImage>();
        private int scoresRequired;
        private bool scoring;

        public Competition(string competitionFileName, string competitionDirectory, string clubName, string trophyName, string competitionKey, string resultsKey) : base(competitionFileName, competitionDirectory, clubName, trophyName, competitionKey, resultsKey)
        {
        }

        internal List<CompetitionImage> GetAllScoredImages()
        {
            string sql = "SELECT timestamp, name, score FROM scores ORDER BY score DESC";
            return SelectScoredImages(sql);
        }

        internal List<CompetitionImage> SelectScoredImages(string sql)
        {
            List<CompetitionImage> scoredImages = new List<CompetitionImage>();

            string databaseFilePath = ImagePaths.GetDatabaseFile(this.GetName());
            SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");
            dbConnection.Open();

            SQLiteCommand cmd = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var timestamp = reader.GetString(0);
                    var imageName =reader.GetString(1);
                    var score = reader.GetInt16(2);

                    foreach (CompetitionImage eachImage in this.images)
                    {
                        if (eachImage.GetFilePath() == imageName)
                        {
                            eachImage.SetScore(score, timestamp);
                            scoredImages.Add(eachImage);
                            break;
                        }
                    }
                }
            }
            return scoredImages;
        }

        internal void SetScoring(bool scoring)
        {
            this.scoring = scoring;
        }

        internal void SetImages(List<CompetitionImage> imageList)
        {
            this.images = imageList;
        }

        internal override bool ScoringEnabled()
        {
            return this.scoring;
        }

        /** Return images with results if there are any, or return all the held images if there aren't */
        internal override List<CompetitionImage> GetSlideshowImages()
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
                                eachImage.SetResult(null);
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

        internal override int GetScoresRequired()
        {
            return this.scoresRequired;
        }

        internal int MaxImageIndex()
        {
            return this.images.Count - 1;
        }

        internal CompetitionImage GetImageObject(int imageIndex)
        {
            return this.images[imageIndex];
        }

        internal void SetScoresRequired(int scoresRequired)
        {
            this.scoresRequired = scoresRequired;
        }

        internal override CompetitionImage GetImageObjectById(string imageId)
        {
            foreach (CompetitionImage eachImage in this.images)
            {
                if (eachImage.GetFilePath() == imageId)
                {
                    return eachImage;
                }
            }
            throw new NotImplementedException();
        }

        internal override CompetitionPanel GetImagePanelById(string panelId)
        {
            throw new NotImplementedException();
        }
    }
}
