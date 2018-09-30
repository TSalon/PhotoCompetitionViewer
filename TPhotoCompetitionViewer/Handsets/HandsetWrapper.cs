using BuzzIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPhotoCompetitionViewer.Competitions;

namespace TPhotoCompetitionViewer.Handsets
{
    class HandsetWrapper
    {
        private readonly List<IBuzzHandsetDevice> handsets;

        public HandsetWrapper(List<IBuzzHandsetDevice> list)
        {
            this.handsets = list;
        }

        internal bool HasSecondHandsetGroup()
        {
            if (this.handsets.Count > 1)
            {
                return true;
            }
            return false;
        }

        /** Turn off all handset lights */
        internal void AllLightsOff()
        {
            this.handsets[0].SetLights(false, false, false, false);
            if (this.HasSecondHandsetGroup())
            {
                this.handsets[1].SetLights(false, false, false, false);
            }
        }

        internal void SetLightsForThisImage(CompetitionImage competitionImage)
        {
            this.AllLightsOff();

            this.handsets[0].SetLights(competitionImage.GetLightStatus(0, 0),
                                       competitionImage.GetLightStatus(0, 1),
                                       competitionImage.GetLightStatus(0, 2),
                                       competitionImage.GetLightStatus(0, 3));

            if (this.HasSecondHandsetGroup())
            {
                this.handsets[1].SetLights(competitionImage.GetLightStatus(1, 0),
                                           competitionImage.GetLightStatus(1, 1),
                                           competitionImage.GetLightStatus(1, 2),
                                           competitionImage.GetLightStatus(1, 3));
            }
        }

        internal IBuzzHandsetDevice Get(int handsetGroup)
        {
            return this.handsets[handsetGroup];
        }
    }
}
