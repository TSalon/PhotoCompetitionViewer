﻿using System;
using System.Collections;
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
    class CompetitionImage
    {
        private readonly string imageTitle; // Lone Tree
        private readonly string imageAuthor; // Tim Sawyer
        private readonly string imagePath; // Tim Sawyer/221_1_Lone Tree.jpg
        private readonly string imageFilename; // 221_1_Lone Tree.jpg
        private readonly IDictionary<String, int> handsetScores = new Dictionary<string,int>(); // Dictionary of handset id -> score
        private readonly Competition competition;
        private string displayResult = "";
        
        public CompetitionImage(Competition competition, XmlNode imageNode)
        {
            string imagePath = imageNode.InnerText;

            this.competition = competition;

            string[] pathParts = Regex.Split(imagePath, @"/");
            string imageAuthor = pathParts[0];
            string imageFilename = pathParts[1];
            string[] imageFilenameParts = Regex.Split(imageFilename, @"_");
            string memberNumber = imageFilenameParts[0];
            string imageNumber = imageFilenameParts[1];
            string imageTitle = imageFilenameParts[2];
            if (imageFilenameParts.Length > 2)
            {
                for (int i = 3; i < imageFilenameParts.Length; i++)
                {
                    imageTitle += " " + imageFilenameParts[i];
                }
            }
            imageTitle = imageTitle.Substring(0, imageTitle.LastIndexOf("."));

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

        internal String GetFullFilePath()
        {
            return this.competition.GetCompetitionDirectory() + "/" + this.GetFilePath();
        }

        internal bool IsHeld(SQLiteConnection dbConnection)
        {
            dbConnection.Open();

            bool isHeld = false;
            string isHeldSql = "SELECT COUNT(*) FROM held_images WHERE name = @name";

            SQLiteCommand cmd = new SQLiteCommand(isHeldSql, dbConnection);
            cmd.Parameters.Add(new SQLiteParameter("@name", this.imageAuthor + "/" + this.imageFilename));
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    isHeld = reader.GetInt16(0) > 0;
                }
            }
            reader.Close();


            dbConnection.Close();

            return isHeld;
        }

        internal string GetAuthor()
        {
            return this.imageAuthor;
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
            }
        }

        internal string GetResult()
        {
            return this.displayResult;
        }

        internal bool ToggleHeld(SQLiteConnection dbConnection)
        {
            bool isHeld = this.IsHeld(dbConnection);

            dbConnection.Open();

            if (isHeld == false)
            {
                // Not held - hold it in the database
                String sql = "INSERT INTO held_images (timestamp, name) VALUES (CURRENT_TIMESTAMP, @name)";
                
                SQLiteCommand insertHeld = new SQLiteCommand(sql, dbConnection);
                insertHeld.Parameters.Add(new SQLiteParameter("@name", this.imageAuthor + "/" + this.imageFilename));
                insertHeld.ExecuteNonQuery();

                this.WriteImageToHeldDirectory();
            }
            else
            {
                // Already held - remove it from the database
                String sql = "DELETE FROM held_images WHERE name = @name";
                SQLiteCommand deleteHeld = new SQLiteCommand(sql, dbConnection);
                deleteHeld.Parameters.Add(new SQLiteParameter("@name", this.imageAuthor + "/" + this.imageFilename));
                deleteHeld.ExecuteNonQuery();

                this.DeleteImageFromHeldDirectory();
            }

            dbConnection.Close();
            return !isHeld;
        }

        private void WriteImageToHeldDirectory()
        {
            String heldDirectory = ImagePaths.GetHeldDirectory(this.competition.GetName());
            if (Directory.Exists(heldDirectory) == false)
            {
                Directory.CreateDirectory(heldDirectory);
            }
            string source = ImagePaths.GetExtractDirectory(this.competition.GetName()) + "/" + this.imagePath;
            string destination = heldDirectory + "/" + this.imageFilename;
            File.Copy(source, destination, true);
        }

        private void DeleteImageFromHeldDirectory()
        {
            String heldDirectory = ImagePaths.GetHeldDirectory(this.competition.GetName());
            string heldImageFile = heldDirectory + "/" + this.imageFilename;
            File.Delete(heldImageFile);
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
            if (this.handsetScores.ContainsKey(key)) return true;
            return false;
        }
    }
}
