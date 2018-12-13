using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPhotoCompetitionViewer.Competitions
{
    class PanelCompetition : AbstractCompetition
    {
        private List<CompetitionPanel> panels;

        public PanelCompetition(string competitionFileName, string competitionDirectory, string clubName, string trophyName) : base(competitionFileName, competitionDirectory, clubName, trophyName)
        {
        }

        internal void SetPanels(List<CompetitionPanel> panelList)
        {
            this.panels = panelList;
        }

        internal CompetitionImage GetImageObject(int panelIndex, int imageIndex)
        {
            CompetitionPanel panel = this.panels[panelIndex];
            CompetitionImage image = panel.GetImage(imageIndex);
            return image;
        }

        internal int MaxPanelIndex()
        {
            return this.panels.Count- 1;
        }

        internal int MaxImageIndex(int panelIndex)
        {
            CompetitionPanel panel = this.panels[panelIndex];
            return panel.MaxImageIndex();
        }

        internal CompetitionPanel GetPanel(int panelIndex)
        {
            return this.panels[panelIndex];
        }

        internal override CompetitionPanel GetImagePanelById(string panelId)
        {
            foreach (CompetitionPanel eachPanel in this.panels)
            {
                if (eachPanel.GetPanelId() == panelId)
                {
                    return eachPanel;
                }
            }
            throw new NotImplementedException();
        }

        internal override CompetitionImage GetImageObjectById(string imageId)
        {
            throw new NotImplementedException();
        }
    }
}
