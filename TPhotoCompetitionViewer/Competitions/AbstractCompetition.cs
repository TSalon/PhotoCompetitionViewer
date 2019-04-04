using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPhotoCompetitionViewer.Competitions
{
    abstract class AbstractCompetition
    {
        private readonly string competitionFileName;
        private readonly string competitionDirectory;
        private readonly string clubName;
        private readonly string trophyName;
        private readonly string competitionKey;
        private readonly string resultsKey;

        public AbstractCompetition(string competitionFileName, string competitionDirectory, string clubName, string trophyName, string competitionKey, string resultsKey)
        {
            this.competitionFileName = competitionFileName;
            this.competitionDirectory = competitionDirectory;
            this.clubName = clubName;
            this.trophyName = trophyName;
            this.competitionKey = competitionKey;
            this.resultsKey = resultsKey;
        }

        internal virtual int GetScoresRequired()
        {
            return 0;
        }

        internal string GetCompetitionDirectory()
        {
            return this.competitionDirectory;
        }


        internal object GetClubName()
        {
            return this.clubName;
        }

        internal object GetTrophyName()
        {
            return this.trophyName;
        }

        internal string GetName()
        {
            return this.competitionFileName;
        }

        internal virtual bool ScoringEnabled()
        {
            return false;
        }

        internal abstract CompetitionImage GetImageObjectById(string imageId);
        internal abstract CompetitionPanel GetImagePanelById(string panelId);
        internal abstract List<CompetitionImage> GetSlideshowImages();
    }
}
