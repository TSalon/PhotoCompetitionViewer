using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPhotoCompetitionViewer.Competitions
{
    class PanelCompetition : AbstractCompetition
    {
        private List<CompetitionPanel> panels;

        public PanelCompetition(string competitionFileName, string competitionDirectory, string clubName, string trophyName, string competitionKey, string resultsKey) : base(competitionFileName, competitionDirectory, clubName, trophyName, competitionKey, resultsKey)
        {
        }

        internal void SetPanels(List<CompetitionPanel> panelList)
        {
            this.panels = panelList;
        }

        internal CompetitionImage GetImageObject(int panelIndex, int imageIndex)
        {
            CompetitionPanel panel = this.panels[panelIndex];
            CompetitionImage image = panel.GetImage(imageIndex);
            return image;
        }

        internal int MaxPanelIndex()
        {
            return this.panels.Count- 1;
        }

        internal int MaxImageIndex(int panelIndex)
        {
            CompetitionPanel panel = this.panels[panelIndex];
            return panel.MaxImageIndex();
        }

        internal CompetitionPanel GetPanel(int panelIndex)
        {
            return this.panels[panelIndex];
        }

        internal override CompetitionPanel GetImagePanelById(string panelId)
        {
            foreach (CompetitionPanel eachPanel in this.panels)
            {
                if (eachPanel.GetPanelId() == panelId)
                {
                    return eachPanel;
                }
            }
            throw new NotImplementedException();
        }

        internal override CompetitionImage GetImageObjectById(string imageId)
        {
            throw new NotImplementedException();
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
                    var panelId = winners_reader.GetString(0);
                    var result = winners_reader.GetString(1);

                    foreach (CompetitionPanel eachPanel in this.panels)
                    {
                        if (eachPanel.GetPanelId() == panelId)
                        {
                            foreach (CompetitionImage eachImage in eachPanel.GetImages())
                            {
                                eachImage.SetResult(result);
                                awardedImages.Add(eachImage);
                            }
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
                        var panelId = held_reader.GetString(0);

                        foreach (CompetitionPanel eachPanel in this.panels)
                        {
                            if (eachPanel.GetPanelId() == panelId)
                            {
                                foreach (CompetitionImage eachImage in eachPanel.GetImages())
                                {
                                    eachImage.SetResult(null);
                                    awardedImages.Add(eachImage);
                                }
                                break;
                            }
                        }
                    }
                }
            }

            dbConnection.Close();

            return awardedImages;
        }
    }
}
