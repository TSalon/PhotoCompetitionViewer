using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TPhotoCompetitionViewer.Competitions;

namespace TPhotoCompetitionViewerTests.Competitions
{
    [TestClass]
    public class ImagePathsTest
    {
        [TestMethod]
        public void TestGetExtractDirectory()
        {
            string competitionName = "2018-12-25 Test Competition";
            string extractDirectory = ImagePaths.GetExtractDirectory(competitionName);

            Assert.AreEqual("c:/CompetitionsExtract/extract/2018-12-25 Test Competition/all", extractDirectory);
        }

        [TestMethod]
        public void TestRemoveSourcePathStart()
        {
            string extractDirectory = ImagePaths.RemoveSourcePathStart("c:/Competitions/extract/2018-12-25 Test Competition/all");
            Assert.AreEqual("extract/2018-12-25 Test Competition/all", extractDirectory);
        }

        [TestMethod]
        public void TestGetZipFile()
        {
            string extractDirectory = ImagePaths.GetZipFile("2018-12-25 Test Competition");
            Assert.AreEqual("c:/Competitions/2018-12-25 Test Competition.zip", extractDirectory);
        }

        [TestMethod]
        public void TestGetCompetitionsDirectory()
        {
            string directory = ImagePaths.GetCompetitionsDirectory();
            Assert.AreEqual("c:/Competitions", directory);
        }

        [TestMethod]
        public void TestGetHeldDirectory()
        {
            string competitionName = "2018-12-25 Test Competition";
            string directory = ImagePaths.GetHeldDirectory(competitionName);

            Assert.AreEqual("c:/CompetitionsExtract/extract/2018-12-25 Test Competition/held", directory);
        }

        [TestMethod]
        public void TestGetDatabaseDirectory()
        {
            string competitionName = "2018-12-25 Test Competition";
            string directory = ImagePaths.GetDatabaseDirectory(competitionName);

            Assert.AreEqual("c:/CompetitionsExtract/extract/2018-12-25 Test Competition/db", directory);
        }

        [TestMethod]
        public void TestGetDatabaseFile()
        {
            string competitionName = "2018-12-25 Test Competition";
            string directory = ImagePaths.GetDatabaseFile(competitionName);

            Assert.AreEqual("c:/CompetitionsExtract/extract/2018-12-25 Test Competition/db/2018-12-25 Test Competition.sqlite", directory);
        }

    }
}
