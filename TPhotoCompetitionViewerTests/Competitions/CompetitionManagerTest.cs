using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TPhotoCompetitionViewer.Competitions;
using TPhotoCompetitionViewer.Properties;

namespace TPhotoCompetitionViewerTests.Competitions
{
    [TestClass]
    public class CompetitionManagerTest
    {
        [TestInitialize]
        public void Setup()
        {
            // Get hold of settings, then change them
            Settings.Default.Reload();
            var extractDir = Settings.Default.CompetitionExtractDir;
            var srcDir = Settings.Default.CompetitionSrcDir;

            Settings.Default.PropertyValues["CompetitionExtractDir"].PropertyValue = "./TestData";
            Settings.Default.PropertyValues["CompetitionSrcDir"].PropertyValue = "./TestCompetitionZip";
        }

        [TestMethod]
        public void TestGetCompetitions()
        {
            CompetitionManager competitionMgr = new CompetitionManager();
            List<string> competitionsList = competitionMgr.GetCompetitions();
            Assert.AreEqual(1, competitionsList.Count);
            Assert.AreEqual("2018-12-25 Test Competition", competitionsList[0]);
        }
    }
}
