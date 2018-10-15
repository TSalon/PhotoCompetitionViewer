using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPhotoCompetitionViewer.Competitions
{
    class CompetitionImage
    {
        private readonly string imageTitle; // Lone Tree
        private readonly string imageAuthor; // Tim Sawyer
        private readonly string imagePath; // Tim Sawyer/221_1_Lone Tree.jpg
        private readonly string imageFilename; // 221_1_Lone Tree.jpg
        private readonly IDictionary<String, int> imageScores = new Dictionary<string,int>();
        private readonly Competition competition;
        
        public CompetitionImage(Competition competition, string imageTitle, string imageAuthor, string imagePath, string imageFilename)
        {
            this.competition = competition;
            this.imageTitle = imageTitle;
            this.imageAuthor = imageAuthor;
            this.imagePath = imagePath;
            this.imageFilename = imageFilename;
        }

        internal string GetFilePath()
        {
            return this.imagePath;
        }

        internal string GetTitle()
        {
            return this.imageTitle;
        }

        internal string GetFilename()
        {
            return this.imageFilename;
        }

        internal bool ToggleHeld(SQLiteConnection dbConnection)
        {
            dbConnection.Open();

            bool isHeld = false;
            string isHeldSql = "SELECT COUNT(*) FROM held_images WHERE name = @name";

            SQLiteCommand cmd = new SQLiteCommand(isHeldSql, dbConnection);
            cmd.Parameters.Add(new SQLiteParameter("@name", this.GetFilename()));
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    isHeld = reader.GetInt16(0) > 0;
                }
            }

            if (isHeld == false)
            {
                // Not held - hold it in the database
                String sql = "INSERT INTO held_images (timestamp, name) VALUES (CURRENT_TIMESTAMP, @name)";
                
                SQLiteCommand insertHeld = new SQLiteCommand(sql, dbConnection);
                insertHeld.Parameters.Add(new SQLiteParameter("@name", this.GetFilename()));
                insertHeld.ExecuteNonQuery();
            }
            else
            {
                // Already held - remove it from the database
                String sql = "DELETE FROM held_images WHERE name = @name";
                SQLiteCommand insertHeld = new SQLiteCommand(sql, dbConnection);
                insertHeld.Parameters.Add(new SQLiteParameter("@name", this.GetFilename()));
                insertHeld.ExecuteNonQuery();
            }

            dbConnection.Close();
            return isHeld;
        }

        internal int ScoreImage(string handsetId, int score, SQLiteConnection dbConnection)
        {
            // record score
            if (score == 0)
            {
                this.imageScores.Remove(handsetId);
            }
            else
            {
                this.imageScores[handsetId] = score;
            }

            bool CompleteCriteraMet = this.imageScores.Keys.Count == this.competition.GetScoresRequired();

            // We have a complete score, write it to the database
            if (CompleteCriteraMet)
            {
                // Calculate total score
                int totalScore = 0;
                foreach(string key in this.imageScores.Keys)
                {
                    int eachScore = this.imageScores[key];
                    totalScore += eachScore;
                }

                // Write score to database
                String sql = "INSERT INTO scores (timestamp, name, score) VALUES (CURRENT_TIMESTAMP, @name, @score)";
		        dbConnection.Open();
                SQLiteCommand insertScore = new SQLiteCommand(sql, dbConnection);
                insertScore.Parameters.Add(new SQLiteParameter("@name", this.GetFilename()));
                insertScore.Parameters.Add(new SQLiteParameter("@score", totalScore));
                insertScore.ExecuteNonQuery();
		        dbConnection.Close();

                return totalScore;
            }

            return 0;
        }

        internal bool GetLightStatus(int handsetGroup, int handsetNumber)
        {
            String key = handsetGroup + "_" + handsetNumber;
            if (this.imageScores.ContainsKey(key)) return true;
            return false;
        }
    }
}
