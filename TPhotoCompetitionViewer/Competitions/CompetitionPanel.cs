﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TPhotoCompetitionViewer.Competitions
{
    class CompetitionPanel
    {
        private readonly AbstractCompetition competition;
        private readonly XmlNode eachPanelNode;
        private readonly int panelPosition;
        private readonly string panelAuthor;
        private readonly string panelId;
        private readonly List<CompetitionImage> images = new List<CompetitionImage>();
        private CompetitionImage panelImage;

        public CompetitionPanel(AbstractCompetition competition, XmlNode eachPanelNode, int panelPosition)
        {
            this.competition = competition;
            this.eachPanelNode = eachPanelNode;
            this.panelPosition = panelPosition;
            this.panelAuthor = eachPanelNode.Attributes["author"].InnerText;
            this.panelId = panelAuthor + "_" + panelPosition + ".jpg";
        }

        internal void AddImage(CompetitionImage eachImage)
        {
            this.images.Add(eachImage);
        }

        internal CompetitionImage GetImage(int imageIndex)
        {
            return this.images[imageIndex];
        }

        internal int MaxImageIndex()
        {
            return this.images.Count - 1;
        }

        internal void ToggleHeld(SQLiteConnection dbConnection)
        {
            HoldTools.ToggleHeld(dbConnection, this.panelId);
        }

        internal bool IsHeld(SQLiteConnection dbConnection)
        {
            return HoldTools.IsHeld(dbConnection, this.panelId);
        }

        internal string GetAuthor()
        {
            return this.panelAuthor;
        }

        internal int GetPosition()
        {
            return this.panelPosition;
        }

        internal string GetPanelId()
        {
            return this.panelId;
        }

        // Create a CompetitionImage that represents the Panel Image
        internal CompetitionImage GetPanelImage()
        {
            if (this.panelImage == null)
            {
                string panelTitle = "Panel " + this.GetPosition();
                CompetitionImage competitionImage = new CompetitionImage(this.competition, this.GetPanelId(), panelTitle, this.panelAuthor, this.GetPosition());
                this.panelImage = competitionImage;
            }
            return this.panelImage;
        }

        internal IEnumerable<CompetitionImage> GetImages()
        {
            return this.images;
        }
    }
}
