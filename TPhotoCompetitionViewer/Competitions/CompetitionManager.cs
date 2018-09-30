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
                var competitionFileName = ImagePaths.RemovePathStart(item);
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

            if (File.Exists(databaseFilePath))
            {
                File.Move(databaseFilePath, databaseFilePath + "." + DateTime.Now.ToString("yyyyMMddhhmmss") + ".sqlite");
            }

            SQLiteConnection.CreateFile(databaseFilePath);

            SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");
            dbConnection.Open();

            String createTableCommandString = "CREATE TABLE scores (timestamp TEXT, name VARCHAR(50) NOT NULL, score NUMBER(2) not null)";
            SQLiteCommand createTableCommand = new SQLiteCommand(createTableCommandString, dbConnection);
            createTableCommand.ExecuteNonQuery();

            dbConnection.Close();
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
