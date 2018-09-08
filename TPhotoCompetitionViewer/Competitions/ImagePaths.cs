using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TPhotoCompetitionViewer.Competitions
{
    class ImagePaths
    {
        private const string BASE_DIRECTORY = "/Competitions";
        private const string EXTRACT_OFFSET = "extract";
        private const string ALL_OFFSET = "all";
        private const string HELD_OFFSET = "held";
        
        internal static string getExtractDirectory(string competitionName)
        {
            return BASE_DIRECTORY + "/" + EXTRACT_OFFSET + "/" + competitionName + "/" + ALL_OFFSET;
        }

        internal static IEnumerable<string> GetCompetitionZipFilesList()
        {
            return Directory.GetFiles(BASE_DIRECTORY);
        }

        internal static string RemovePathStart(string item)
        {
            return item.Substring(ImagePaths.BASE_DIRECTORY.Length + 1);
        }

        internal static string GetZipFile(string competitionName)
        {
            return BASE_DIRECTORY + "/" + competitionName + ".zip";
        }

        internal static string GetHeldDirectory(string competitionName)
        {
            return BASE_DIRECTORY + "/" + EXTRACT_OFFSET + "/" + competitionName + "/" + HELD_OFFSET;
        }
    }

   
}
