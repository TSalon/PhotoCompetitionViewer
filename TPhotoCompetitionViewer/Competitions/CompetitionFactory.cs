using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TPhotoCompetitionViewer.Competitions
{
    class CompetitionFactory
    {
        private const string CONTROL_FILENAME = "competition.xml";

        internal static AbstractCompetition Load(string competitionFileName)
        {
            string competitionDirectory = ImagePaths.GetExtractDirectory(competitionFileName);
            AbstractCompetition competition = null;

            try
            {
                // Load image order details
                XmlDocument orderingDocument = new XmlDocument();
                orderingDocument.Load(competitionDirectory + "/" + CONTROL_FILENAME);
                XmlNode rootNode = orderingDocument.FirstChild;
                string competitionStyle = rootNode["Style"].InnerText;
                string clubName = rootNode["Club"].InnerText;
                string trophyName = rootNode["Trophy"].InnerText;
                bool scoring = rootNode["Scoring"].InnerText.ToLower() == "true";

                switch (competitionStyle.ToLower())
                {
                    case "panel":
                        competition = new PanelCompetition(competitionFileName, competitionDirectory, clubName, trophyName);
                        XmlNode panelsNode = rootNode["Panels"];
                        int panelPosition = 1;
                        var panelList = new List<CompetitionPanel>();
                        foreach (XmlNode eachPanelNode in panelsNode.ChildNodes)
                        {
                            CompetitionPanel eachPanel = new CompetitionPanel(competition, eachPanelNode, panelPosition);

                            int imagePosition = 1;
                            foreach (XmlNode eachImageNode in eachPanelNode.ChildNodes)
                            {
                                CompetitionImage eachImage = new CompetitionImage(competition, eachImageNode, imagePosition);
                                eachPanel.AddImage(eachImage);
                                imagePosition++;
                            }


                            panelList.Add(eachPanel);
                            panelPosition++;
                        }
                        ((PanelCompetition)competition).SetPanels(panelList);
                        break;

                    default:
                        competition = new Competition(competitionFileName, competitionDirectory, clubName, trophyName);
                        XmlNode imagesNode = rootNode["Images"];
                        int i = 1;
                        var imageList = new List<CompetitionImage>();
                        foreach (XmlNode eachImageNode in imagesNode.ChildNodes)
                        {
                            CompetitionImage eachImage = new CompetitionImage(competition, eachImageNode, i);
                            imageList.Add(eachImage);
                            i += 1;
                        }

                        ((Competition)competition).SetImages(imageList);
                        ((Competition)competition).SetScoring(scoring);
                        break;
                }
            }
            catch (Exception e)
            {
                string clubName = "Competition zip file broken";
                string trophyName = e.Message;
                competition = new Competition(competitionFileName, competitionDirectory, clubName, trophyName);
            }

            return competition;

        }
    }
}
