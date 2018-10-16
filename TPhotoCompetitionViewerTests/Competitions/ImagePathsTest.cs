using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TPhotoCompetitionViewer.Competitions;
using TPhotoCompetitionViewer.Properties;

namespace TPhotoCompetitionViewerTests.Competitions
{
    [TestClass]
    public class ImagePathsTest
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
        public void TestGetExtractDirectory()
        {
            string competitionName = "2018-12-25 Test Competition";
            string extractDirectory = ImagePaths.GetExtractDirectory(competitionName);

            Assert.AreEqual("./TestData/extract/2018-12-25 Test Competition/all", extractDirectory);
        }

        [TestMethod]
        public void TestRemoveSourcePathStart()
        {
            string extractDirectory = ImagePaths.RemoveSourcePathStart("./TestCompetitionZip/2018-12-25 Test Competition.zip");
            Assert.AreEqual("2018-12-25 Test Competition.zip", extractDirectory);
        }

        [TestMethod]
        public void TestGetZipFile()
        {
            string extractDirectory = ImagePaths.GetZipFile("2018-12-25 Test Competition");
            Assert.AreEqual("./TestCompetitionZip/2018-12-25 Test Competition.zip", extractDirectory);
        }

        [TestMethod]
        public void TestGetCompetitionsDirectory()
        {
            string directory = ImagePaths.GetCompetitionsDirectory();
            Assert.AreEqual("./TestCompetitionZip", directory);
        }

        [TestMethod]
        public void TestGetHeldDirectory()
        {
            string competitionName = "2018-12-25 Test Competition";
            string directory = ImagePaths.GetHeldDirectory(competitionName);

            Assert.AreEqual("./TestData/extract/2018-12-25 Test Competition/held", directory);
        }

        [TestMethod]
        public void TestGetDatabaseDirectory()
        {
            string competitionName = "2018-12-25 Test Competition";
            string directory = ImagePaths.GetDatabaseDirectory(competitionName);

            Assert.AreEqual("./TestData/extract/2018-12-25 Test Competition/db", directory);
        }

        [TestMethod]
        public void TestGetDatabaseFile()
        {
            string competitionName = "2018-12-25 Test Competition";
            string directory = ImagePaths.GetDatabaseFile(competitionName);

            Assert.AreEqual("./TestData/extract/2018-12-25 Test Competition/db/2018-12-25 Test Competition.sqlite", directory);
        }
    }
}
