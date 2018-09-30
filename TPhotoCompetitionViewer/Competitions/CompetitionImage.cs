using System;
using System.Collections;
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
        private readonly IDictionary<String, int> imageScores = new Dictionary<string,int>();
        
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

        internal void ScoreImage(string handsetId, int score)
        {
            if (score == 0)
            {
                this.imageScores.Remove(handsetId);
            }
            else
            {
                this.imageScores[handsetId] = score;
            }
        }

        internal bool GetLightStatus(int handsetGroup, int handsetNumber)
        {
            String key = handsetGroup + "_" + handsetNumber;
            if (this.imageScores.ContainsKey(key)) return true;
            return false;
        }
    }
}
