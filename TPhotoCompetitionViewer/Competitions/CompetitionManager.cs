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
        List<string> competitionList = new List<String>();

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
                    this.competitionList.Add(competitionName);
                }
            }
            return competitionList;
        }

        internal Competition GetCompetition(int competitionIndex)
        {
            string competitionName = this.competitionList[competitionIndex];
            Competition competition = new Competition(competitionName);
            competition.LoadImages(ImagePaths.getExtractDirectory(competitionName));
            return competition;
        }

        /** Extract from zip file to tmp directory */
        private void ExtractFiles(string competitionName)
        {
            string zipFilePath = ImagePaths.GetZipFile(competitionName);
            string destination = ImagePaths.getExtractDirectory(competitionName);
            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }
            ZipFile.ExtractToDirectory(zipFilePath, destination);
        }
    }


}
