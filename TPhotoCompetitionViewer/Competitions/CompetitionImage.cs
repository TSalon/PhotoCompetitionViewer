using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPhotoCompetitionViewer.Competitions
{
    class CompetitionImage
    {
        private readonly string imageTitle;
        private readonly string imageAuthor;
        private readonly string imagePath;
        private bool held = false;

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

        internal string GetTitle()
        {
            return imageTitle;
        }

        internal bool ToggleHeld()
        {
            this.held = !this.held;
            return this.held;
        }
    }
}
