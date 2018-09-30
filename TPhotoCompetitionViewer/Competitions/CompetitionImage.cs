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
        private readonly string imageTitle; // Lone Tree
        private readonly string imageAuthor; // Tim Sawyer
        private readonly string imagePath; // Tim Sawyer/221_1_Lone Tree.jpg
        private readonly string imageFilename; // 221_1_Lone Tree.jpg
        private bool held = false;
        private readonly IDictionary<String, int> imageScores = new Dictionary<string,int>();
        
        public CompetitionImage(string imageTitle, string imageAuthor, string imagePath, string imageFilename)
        {
            this.imageTitle = imageTitle;
            this.imageAuthor = imageAuthor;
            this.imagePath = imagePath;
            this.imageFilename = imageFilename;
        }

        internal string GetFilePath()
        {
            return this.imagePath;
        }

        internal string GetTitle()
        {
            return this.imageTitle;
        }

        internal string getFilename()
        {
            return this.imageFilename;
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
