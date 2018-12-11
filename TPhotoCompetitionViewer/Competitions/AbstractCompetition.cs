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

        public AbstractCompetition(string competitionFileName, string competitionDirectory, string clubName, string trophyName)
        {
            this.competitionFileName = competitionFileName;
            this.competitionDirectory = competitionDirectory;
            this.clubName = clubName;
            this.trophyName = trophyName;
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
    }
}
