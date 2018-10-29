using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
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

            // delete database.  Test will recreate it when reading the competition zip file
            String databaseDir = ImagePaths.GetDatabaseDirectory("2018-12-25 Test Competition");
            if (Directory.Exists(databaseDir))
            {
                String databaseFile = ImagePaths.GetDatabaseFile("2018-12-25 Test Competition");
                File.Delete(databaseFile);
            }
        }

        [TestMethod]
        public void TestCompetitions()
        {
            // Test competition manager method
            List<string> competitionsList = CompetitionHelper.GetCompetitions();
            Assert.AreEqual(1, competitionsList.Count);
            Assert.AreEqual("2018-12-25 Test Competition", competitionsList[0]);

            List<string> heldImages = CompetitionHelper.GetHeldImages("2018-12-25 Test Competition");
            Assert.AreEqual(0, heldImages.Count);

            int heldImageCount = CompetitionHelper.FetchHeldImageCount("2018-12-25 Test Competition");
            Assert.AreEqual(0, heldImageCount);

            // Get hold of competition object and look at methods on that
            Competition competition = CompetitionHelper.GetCompetition(0, 3);
            Assert.AreEqual("2018-12-25 Test Competition", competition.GetName());
            Assert.AreEqual(3, competition.GetScoresRequired());

            Assert.AreEqual(2, competition.MaxImageIndex());  // 3 images = 0, 1, 2.
            Assert.AreEqual("./TestData/extract/2018-12-25 Test Competition/all/Tim Sawyer/221_2_Reflective.jpg", competition.GetImageObject(0).GetFullFilePath());
            Assert.AreEqual("./TestData/extract/2018-12-25 Test Competition/all/Tim Sawyer/221_1_Bridgewater.jpg", competition.GetImageObject(1).GetFullFilePath());
            Assert.AreEqual("./TestData/extract/2018-12-25 Test Competition/all/Tim Sawyer/221_3_Young Red_Kite.jpg", competition.GetImageObject(2).GetFullFilePath());

            // Get hold of the first three images and look at the methods there
            CompetitionImage firstImage = competition.GetImageObject(0);
            Assert.AreEqual("221_2_Reflective.jpg", firstImage.GetFilename());
            Assert.AreEqual("Tim Sawyer/221_2_Reflective.jpg", firstImage.GetFilePath());
            Assert.AreEqual("Reflective", firstImage.GetTitle());

            CompetitionImage secondImage = competition.GetImageObject(1);
            Assert.AreEqual("221_1_Bridgewater.jpg", secondImage.GetFilename());
            Assert.AreEqual("Tim Sawyer/221_1_Bridgewater.jpg", secondImage.GetFilePath());
            Assert.AreEqual("Bridgewater", secondImage.GetTitle());

            CompetitionImage thirdImage = competition.GetImageObject(2);
            Assert.AreEqual("221_3_Young Red_Kite.jpg", thirdImage.GetFilename());
            Assert.AreEqual("Tim Sawyer/221_3_Young Red_Kite.jpg", thirdImage.GetFilePath());
            Assert.AreEqual("Young Red Kite", thirdImage.GetTitle());

            // Test held functionality
            string databaseFilePath = ImagePaths.GetDatabaseFile(competition.GetName());
            SQLiteConnection dbConnection = new SQLiteConnection("DataSource=" + databaseFilePath + ";Version=3;");

            Assert.AreEqual(false, firstImage.IsHeld(dbConnection));
            Assert.AreEqual(false, secondImage.IsHeld(dbConnection));
            Assert.AreEqual(false, thirdImage.IsHeld(dbConnection));

            // Hold one
            bool thirdHeld = thirdImage.ToggleHeld(dbConnection);
            Assert.AreEqual(true, thirdHeld);
            int heldImageCountAfterHoldingOneImage = CompetitionHelper.FetchHeldImageCount(competition.GetName());
            Assert.AreEqual(1, heldImageCountAfterHoldingOneImage);

            List<string> heldImagesAfterHoldingOneImage = CompetitionHelper.GetHeldImages(competition.GetName());
            Assert.AreEqual(1, heldImagesAfterHoldingOneImage.Count);
            Assert.AreEqual("Tim Sawyer/221_3_Young Red_Kite.jpg", heldImagesAfterHoldingOneImage[0]);

            // Hold another
            bool firstHeld = firstImage.ToggleHeld(dbConnection);
            Assert.AreEqual(true, firstHeld);
            int heldImageCountAfterHoldingTwoImages = CompetitionHelper.FetchHeldImageCount(competition.GetName());
            Assert.AreEqual(2, heldImageCountAfterHoldingTwoImages);

            List<string> heldImagesAfterHoldingTwoImages = CompetitionHelper.GetHeldImages(competition.GetName());
            Assert.AreEqual(2, heldImagesAfterHoldingTwoImages.Count);
            Assert.AreEqual("Tim Sawyer/221_3_Young Red_Kite.jpg", heldImagesAfterHoldingOneImage[0]);
            Assert.AreEqual("Tim Sawyer/221_2_Reflective.jpg", heldImagesAfterHoldingTwoImages[1]);

            // Unhold first one
            bool thirdUnHeld = thirdImage.ToggleHeld(dbConnection);
            Assert.AreEqual(false, thirdUnHeld);
            int heldImageCountAfterUnHoldingImage = CompetitionHelper.FetchHeldImageCount(competition.GetName());
            Assert.AreEqual(1, heldImageCountAfterUnHoldingImage);

            List<string> heldImagesAfterUnHoldingImage = CompetitionHelper.GetHeldImages(competition.GetName());
            Assert.AreEqual(1, heldImagesAfterUnHoldingImage.Count);
            Assert.AreEqual("Tim Sawyer/221_2_Reflective.jpg", heldImagesAfterUnHoldingImage[0]);

            // Hold second one through competition method
            bool held = competition.GetImageObject(1).ToggleHeld(dbConnection);
            Assert.IsTrue(held);

            bool secondHeld = secondImage.IsHeld(dbConnection);
            Assert.AreEqual(true, secondHeld);
            int heldImageCountAfterHoldingThroughCompetition = CompetitionHelper.FetchHeldImageCount(competition.GetName());
            Assert.AreEqual(2, heldImageCountAfterHoldingThroughCompetition);

            List<string> heldImagesAfterHoldingThroughCompetition = CompetitionHelper.GetHeldImages(competition.GetName());
            Assert.AreEqual(2, heldImagesAfterHoldingThroughCompetition.Count);
            Assert.AreEqual("Tim Sawyer/221_2_Reflective.jpg", heldImagesAfterHoldingThroughCompetition[0]);
            Assert.AreEqual("Tim Sawyer/221_1_Bridgewater.jpg", heldImagesAfterHoldingThroughCompetition[1]);

            // Test unholding through competition method
            held = competition.GetImageObject(1).ToggleHeld(dbConnection);
            Assert.IsFalse(held);

            bool secondUnHeld = secondImage.IsHeld(dbConnection);
            Assert.AreEqual(false, secondUnHeld);
            int heldImageCountAfterUnHoldingThroughCompetition = CompetitionHelper.FetchHeldImageCount(competition.GetName());
            Assert.AreEqual(1, heldImageCountAfterUnHoldingThroughCompetition);

            List<string> heldImagesAfterUnHoldingThroughCompetition = CompetitionHelper.GetHeldImages(competition.GetName());
            Assert.AreEqual(1, heldImagesAfterUnHoldingThroughCompetition.Count);
            Assert.AreEqual("Tim Sawyer/221_2_Reflective.jpg", heldImagesAfterUnHoldingThroughCompetition[0]);

            // Test scoring
            int totalScore = firstImage.ScoreImage("0_0", 5, dbConnection);
            Assert.AreEqual(0, totalScore);
            totalScore = firstImage.ScoreImage("0_1", 4, dbConnection);
            Assert.AreEqual(0, totalScore);
            totalScore = firstImage.ScoreImage("0_2", 3, dbConnection);
            Assert.AreEqual(12, totalScore);

            totalScore = secondImage.ScoreImage("1_0", 2, dbConnection);
            Assert.AreEqual(0, totalScore);
            totalScore = secondImage.ScoreImage("0_0", 2, dbConnection);
            Assert.AreEqual(0, totalScore);
            totalScore = secondImage.ScoreImage("1_3", 2, dbConnection);
            Assert.AreEqual(6, totalScore);

            totalScore = thirdImage.ScoreImage("1_0", 5, dbConnection);
            Assert.AreEqual(0, totalScore);
            totalScore = thirdImage.ScoreImage("0_0", 5, dbConnection);
            Assert.AreEqual(0, totalScore);
            totalScore = thirdImage.ScoreImage("1_3", 5, dbConnection);
            Assert.AreEqual(15, totalScore);
        }
    }
}
