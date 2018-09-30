using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPhotoCompetitionViewer.Handsets
{
    class HandsetIdTools
    {
        public static int GetHandsetNumber(String handsetId)
        {
           return System.Convert.ToInt32(handsetId.Substring(2, 1));
        }

        public static int GetHandsetGroup(String handsetId)
        {
            return System.Convert.ToInt32(handsetId.Substring(0, 1));
        }

        public static String BuildHandsetId(int handsetGroup, int handsetPosition)
        {
            return handsetGroup + "_" + handsetPosition;
        }
    }
}
