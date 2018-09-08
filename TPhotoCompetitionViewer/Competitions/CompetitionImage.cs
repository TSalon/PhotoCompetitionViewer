using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPhotoCompetitionViewer.Competitions
{
    class CompetitionImage
    {
        private string imageTitle;
        private string imageAuthor;
        private string imagePath;

        public CompetitionImage(string imageTitle, string imageAuthor, string imagePath)
        {
            this.imageTitle = imageTitle;
            this.imageAuthor = imageAuthor;
            this.imagePath = imagePath;
        }

        internal string GetFilePath()
        {
            return imagePath;
        }
    }
}
