using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TPhotoCompetitionViewer.Competitions
{
    public class ImagePaths
    {
        private static readonly string COMPETITION_SRC_DIRECTORY = Properties.Settings.Default.CompetitionSrcDir;
        private static readonly string COMPETITION_EXTRACT_DIRECTORY = Properties.Settings.Default.CompetitionExtractDir;
        private const string EXTRACT_OFFSET = "extract";
        private const string ALL_OFFSET = "all";
        private const string HELD_OFFSET = "held";
        private const string DATABASE_OFFSET = "db";

        internal static string GetExtractDirectory(string competitionName)
        {
            if (Directory.Exists(COMPETITION_EXTRACT_DIRECTORY) == false)
            {
                Directory.CreateDirectory(COMPETITION_EXTRACT_DIRECTORY);
            }

            return COMPETITION_EXTRACT_DIRECTORY + "/" + EXTRACT_OFFSET + "/" + competitionName + "/" + ALL_OFFSET;
        }

        internal static IEnumerable<string> GetCompetitionZipFilesList()
        {
            if (Directory.Exists(COMPETITION_SRC_DIRECTORY) == false)
            {
                Directory.CreateDirectory(COMPETITION_SRC_DIRECTORY);
            }

            string[] competitionZipFiles = Directory.GetFiles(COMPETITION_SRC_DIRECTORY);
            Array.Sort(competitionZipFiles, StringComparer.InvariantCultureIgnoreCase);
            return competitionZipFiles;
        }

        internal static string RemoveSourcePathStart(string filePath)
        {
            return filePath.Substring(ImagePaths.COMPETITION_SRC_DIRECTORY.Length + 1);
        }

        internal static string GetZipFile(string competitionName)
        {
            return COMPETITION_SRC_DIRECTORY + "/" + competitionName + ".iris";
        }

        internal static string GetCompetitionsDirectory()
        {
            return COMPETITION_SRC_DIRECTORY;
        }

        internal static string GetHeldDirectory(string competitionName)
        {
            return COMPETITION_EXTRACT_DIRECTORY + "/" + EXTRACT_OFFSET + "/" + competitionName + "/" + HELD_OFFSET;
        }

        internal static string GetDatabaseDirectory(string competitionName)
        {
            return COMPETITION_EXTRACT_DIRECTORY + "/" + EXTRACT_OFFSET + "/" + competitionName + "/" + DATABASE_OFFSET;
        }

        internal static string GetDatabaseFile(string competitionName)
        {
            return ImagePaths.GetDatabaseDirectory(competitionName) + "/" + competitionName + ".sqlite";
        }
    }

   
}
