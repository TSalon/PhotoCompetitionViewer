using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPhotoCompetitionViewer.Competitions
{
    class CompetitionManager
    {
        private const string BASE_DIRECTORY = "/Competitions";
        private const string EXTRACT_OFFSET = "extract";
        List<string> competitionList = new List<String>();

        internal List<string> GetCompetitions()
        {
            IEnumerable<string> competitions = Directory.GetFiles(BASE_DIRECTORY);

            foreach (var item in competitions)
            {
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                var competitionFileName = item.Substring(BASE_DIRECTORY.Length + 1);
                var competitionName = competitionFileName.Substring(0, competitionFileName.Length - 4);

                if (competitionName.StartsWith(today)){
                    this.ExtractFiles(competitionName);
                    this.competitionList.Add(competitionName);
                }
            }
            return competitionList;
        }

        internal string getExtractDirectory(string competitionName)
        {
            return BASE_DIRECTORY + "/" + EXTRACT_OFFSET + "/" + competitionName;
        }

        internal Competition GetCompetition(int competitionIndex)
        {
            string competitionName = this.competitionList[competitionIndex];
            Competition competition = new Competition(competitionName);
            competition.LoadImages(this.getExtractDirectory(competitionName));
            return competition;
        }

        /** Extract from zip file to tmp directory */
        private void ExtractFiles(string competitionName)
        {
            string zipFilePath = BASE_DIRECTORY + "/" + competitionName + ".zip";
            string destination = this.getExtractDirectory(competitionName);
            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }
            ZipFile.ExtractToDirectory(zipFilePath, destination);
        }
    }


}
