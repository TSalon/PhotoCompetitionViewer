using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPhotoCompetitionViewer.Competitions
{
    class CompetitionManager
    {
        readonly List<string> competitionList = new List<String>();

        internal List<string> GetCompetitions()
        {
            IEnumerable<string> competitions = ImagePaths.GetCompetitionZipFilesList();

            foreach (var item in competitions)
            {
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                var competitionFileName = ImagePaths.RemoveSourcePathStart(item);
                var competitionName = competitionFileName.Substring(0, competitionFileName.Length - 4);

                if (competitionName.StartsWith(today)){
                    this.ExtractFiles(competitionName);
                    this.CreateDatabase(competitionName);
                    this.competitionList.Add(competitionName);
                }
            }
            return competitionList;
        }

        private void CreateDatabase(string competitionName)
        {
            string databaseDirectory = ImagePaths.GetDatabaseDirectory(competitionName);
            Directory.CreateDirectory(databaseDirectory);

            string databaseFilePath = ImagePaths.GetDatabaseFile(competitionName);

            if (File.Exists(databaseFilePath) == false)
            {
                SQLiteConnection.CreateFile(databaseFilePath);

                SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");
                dbConnection.Open();

                String createScoresTableCommandString = "CREATE TABLE scores (timestamp TEXT, name VARCHAR(50) NOT NULL, score NUMBER(2) not null)";
                SQLiteCommand createScoresTableCommand = new SQLiteCommand(createScoresTableCommandString, dbConnection);
                createScoresTableCommand.ExecuteNonQuery();

                String createHeldImagesTableCommandString = "CREATE TABLE held_images (timestamp TEXT, name VARCHAR(50) NOT NULL)";
                SQLiteCommand createHeldImagesTableCommand = new SQLiteCommand(createHeldImagesTableCommandString, dbConnection);
                createHeldImagesTableCommand.ExecuteNonQuery();
                dbConnection.Close();
            }          
        }

        internal int FetchHeldImageCount(string competitionName)
        {
            int heldImagesCount = 0;

            string databaseFilePath = ImagePaths.GetDatabaseFile(competitionName);
            SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");
            dbConnection.Open();

            string sql = "SELECT count(*) FROM held_images";

            SQLiteCommand cmd = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    heldImagesCount = reader.GetInt16(0); 
                }
            }

            dbConnection.Close();

            return heldImagesCount;
        }
        
        internal Competition GetCompetition(int competitionIndex, int scoresRequired)
        {
            string competitionName = this.competitionList[competitionIndex];
            Competition competition = new Competition(competitionName, scoresRequired);
            competition.LoadImages(ImagePaths.GetExtractDirectory(competitionName));
            return competition;
        }

        /** Extract from zip file to tmp directory */
        private void ExtractFiles(string competitionName)
        {
            string zipFilePath = ImagePaths.GetZipFile(competitionName);
            string destination = ImagePaths.GetExtractDirectory(competitionName);
            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }
            ZipFile.ExtractToDirectory(zipFilePath, destination);
        }
    }


}
