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
    class CompetitionHelper
    {
        private CompetitionHelper()
        {
            // Prevent instantiation
        }

        internal static List<Competition> GetCompetitions()
        {
            IEnumerable<string> competitions = ImagePaths.GetCompetitionZipFilesList();
            List<Competition> competitionList = new List<Competition>();
            foreach (var item in competitions)
            {
                var today = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                var competitionFileName = ImagePaths.RemoveSourcePathStart(item);
                if (competitionFileName.ToLower().EndsWith(".zip"))
                {
                    var competitionDateString = competitionFileName.Substring(0, 10);
                    var competitionDate = DateTime.Parse(competitionDateString);
                    var competitionName = competitionFileName.Substring(0, competitionFileName.Length - 4);

                    if (competitionDate >= today)
                    {
                        CompetitionHelper.ExtractFiles(competitionName);
                        CompetitionHelper.CreateDatabase(competitionName);

                        Competition competition = new Competition(competitionName);
                        competition.LoadImages(ImagePaths.GetExtractDirectory(competition.GetName()));
                        competitionList.Add(competition);
                    }
                }
            }
            return competitionList;
        }

        internal static List<string> GetHeldImages(string competitionName)
        {
            List<string> heldImages = new List<string>();

            string databaseFilePath = ImagePaths.GetDatabaseFile(competitionName);
            SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");
            dbConnection.Open();

            string sql = "SELECT name FROM held_images";

            SQLiteCommand cmd = new SQLiteCommand(sql, dbConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    heldImages.Add(reader.GetString(0));
                }
            }

            dbConnection.Close();

            return heldImages;
        }

        private static void CreateDatabase(string competitionName)
        {
            string databaseDirectory = ImagePaths.GetDatabaseDirectory(competitionName);
            Directory.CreateDirectory(databaseDirectory);

            string databaseFilePath = ImagePaths.GetDatabaseFile(competitionName);

            if (File.Exists(databaseFilePath) == false)
            {
                //SQLiteConnection.CreateFile(databaseFilePath);

                SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");
                dbConnection.Open();

                String createScoresTableCommandString = "CREATE TABLE IF NOT EXISTS scores (timestamp TEXT, name VARCHAR(255) NOT NULL, score NUMBER(2) not null)";
                SQLiteCommand createScoresTableCommand = new SQLiteCommand(createScoresTableCommandString, dbConnection);
                createScoresTableCommand.ExecuteNonQuery();

                String createHeldImagesTableCommandString = "CREATE TABLE IF NOT EXISTS held_images (timestamp TEXT, name VARCHAR(255) NOT NULL)";
                SQLiteCommand createHeldImagesTableCommand = new SQLiteCommand(createHeldImagesTableCommandString, dbConnection);
                createHeldImagesTableCommand.ExecuteNonQuery();

                String createWinnersTableCommandString = "CREATE TABLE IF NOT EXISTS winners (timestamp TEXT, name VARCHAR(255) NOT NULL, position VARCHAR(2) NOT NULL)";
                SQLiteCommand createWinnersTableCommand = new SQLiteCommand(createWinnersTableCommandString, dbConnection);
                createWinnersTableCommand.ExecuteNonQuery();

                dbConnection.Close();
            }          
        }

        internal static int FetchHeldImageCount(string competitionName)
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
        
        internal static Competition GetCompetition(int competitionIndex, int scoresRequired)
        {
            Competition competition = CompetitionHelper.GetCompetitions()[competitionIndex];
            competition.SetScoresRequired(scoresRequired);
            competition.LoadImages(ImagePaths.GetExtractDirectory(competition.GetName()));
            return competition;
        }

        /** Extract from zip file to tmp directory */
        private static void ExtractFiles(string competitionName)
        {
            string zipFilePath = ImagePaths.GetZipFile(competitionName);
            string destination = ImagePaths.GetExtractDirectory(competitionName);
            if (Directory.Exists(destination) == false)
            {
                ZipFile.ExtractToDirectory(zipFilePath, destination);
            }
        }
    }
}
