using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TPhotoCompetitionViewer.Competitions
{
    public class CompetitionImage
    {
        private readonly string imageTitle; // Lone Tree
        private readonly string imageAuthor; // Tim Sawyer
        private readonly int imagePosition; // first one is 1
        private readonly string imagePath; // Tim Sawyer/221_1_Lone Tree.jpg
        private readonly string imageId; // unique id - serial of CompetitionImage in database.  Used for uploading results
        private readonly IDictionary<String, int> handsetScores = new Dictionary<string,int>(); // Dictionary of handset id -> score
        private readonly AbstractCompetition competition;
        private string displayResult = "";
        private short score;
        private string scoreTimestamp;

        internal CompetitionImage(AbstractCompetition competition, string imagePath, string title, string author, int imagePosition)
        {
            this.competition = competition;
            this.imagePosition = imagePosition;

            this.imagePath = imagePath;
            this.imageTitle = title;
            this.imageAuthor = author;
        }

        internal CompetitionImage(AbstractCompetition competition, XmlNode imageNode, int imagePosition)
        {
            this.competition = competition;
            this.imagePosition = imagePosition;

            this.imagePath = imageNode.InnerText;
            this.imageTitle = imageNode.Attributes["title"].InnerText;
            this.imageAuthor = imageNode.Attributes["author"].InnerText;
            this.imageId = imageNode.Attributes["id"].InnerText;
        }

        internal string GetFilePath()
        {
            return this.imagePath;
        }

        internal string GetTitle()
        {
            return this.imageTitle;
        }

        internal object GetScoreTimestamp()
        {
            return this.scoreTimestamp;
        }

        internal String GetFullFilePath()
        {
            return this.competition.GetCompetitionDirectory() + "/" + this.GetFilePath();
        }

        internal bool IsHeld(SQLiteConnection dbConnection)
        {
            return HoldTools.IsHeld(dbConnection, this.imagePath);
        }

       

        internal string GetAuthor()
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(this.imageAuthor);
        }

        internal void SetResult(string result)
        {
            switch (result)
            {
                case "1":
                    this.displayResult = "First";
                    break;
                case "2":
                    this.displayResult = "Second";
                    break;
                case "3":
                    this.displayResult = "Third";
                    break;
                case "HC":
                    this.displayResult = "Highly Commended";
                    break;
                case "C":
                    this.displayResult = "Commended";
                    break;
                default:
                    this.displayResult = "";
                    break;

            }
        }

        internal void SetScore(short score, string timestamp)
        {
            this.score = score;
            this.scoreTimestamp = timestamp;
        }

        internal short GetScore()
        {
            return this.score;
        }

        internal string GetResult()
        {
            return this.displayResult;
        }

        internal bool ToggleHeld(SQLiteConnection dbConnection)
        {
            return HoldTools.ToggleHeld(dbConnection, this.imagePath);
        }

       

        internal int ScoreImage(string handsetId, int score, SQLiteConnection dbConnection)
        {
            // record score
            if (score == 0)
            {
                this.handsetScores.Remove(handsetId);
            }
            else
            {
                this.handsetScores[handsetId] = score;
            }

            bool CompleteCriteraMet = this.handsetScores.Keys.Count == this.competition.GetScoresRequired();

            // We have a complete score, write it to the database
            if (CompleteCriteraMet)
            {
                // Calculate total score
                int totalScore = 0;
                foreach(string key in this.handsetScores.Keys)
                {
                    int eachScore = this.handsetScores[key];
                    totalScore += eachScore;
                }

                // Write score to database
                String sql = "INSERT INTO scores (timestamp, name, author, title, score) VALUES (CURRENT_TIMESTAMP, @name, @author, @title, @score)";
		        dbConnection.Open();
                SQLiteCommand insertScore = new SQLiteCommand(sql, dbConnection);
                insertScore.Parameters.Add(new SQLiteParameter("@name", this.imagePath));
                insertScore.Parameters.Add(new SQLiteParameter("@score", totalScore));
                insertScore.Parameters.Add(new SQLiteParameter("@author", this.imageAuthor));
                insertScore.Parameters.Add(new SQLiteParameter("@title", this.imageTitle));
                insertScore.ExecuteNonQuery();

		        dbConnection.Close();

                return totalScore;
            }

            return 0;
        }

        internal bool GetLightStatus(int handsetGroup, int handsetNumber)
        {
            String key = handsetGroup + "_" + handsetNumber;
            if (this.handsetScores.ContainsKey(key)) return true;
            return false;
        }

        internal int GetIndex()
        {
            return this.imagePosition;
        }
    }
}
