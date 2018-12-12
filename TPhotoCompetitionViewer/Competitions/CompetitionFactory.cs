using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
                        competition = LoadPanelCompetition(competitionFileName, competitionDirectory, rootNode, clubName, trophyName);
                        break;

                    default:
                        competition = LoadSingleImageCompetition(competitionFileName, competitionDirectory, rootNode, clubName, trophyName, scoring);
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

        private static AbstractCompetition LoadPanelCompetition(string competitionFileName, string competitionDirectory, XmlNode rootNode, string clubName, string trophyName)
        {
            AbstractCompetition competition = new PanelCompetition(competitionFileName, competitionDirectory, clubName, trophyName);
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

                CompetitionFactory.WritePanelLayoutImage(competitionFileName, competitionDirectory, eachPanel);

                panelList.Add(eachPanel);
                panelPosition++;
            }
            ((PanelCompetition)competition).SetPanels(panelList);
            return competition;
        }

        private static void WritePanelLayoutImage(string competitionFileName, string competitionDirectory, CompetitionPanel eachPanel)
        {
            // Loads the images to tile (no need to specify PngBitmapDecoder, the correct decoder is automatically selected)
            BitmapFrame frame1 = BitmapDecoder.Create(new Uri(eachPanel.GetImage(0).GetFullFilePath()), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
            BitmapFrame frame2 = BitmapDecoder.Create(new Uri(eachPanel.GetImage(1).GetFullFilePath()), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
            BitmapFrame frame3 = BitmapDecoder.Create(new Uri(eachPanel.GetImage(2).GetFullFilePath()), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();

            // Gets the size of the images (I assume each image has the same size)
            int imageWidth = frame1.PixelWidth;
            int imageHeight = frame1.PixelHeight;

            // Draws the images into a DrawingVisual component
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(frame1, new Rect(0, 0, imageWidth, imageHeight));
                drawingContext.DrawImage(frame2, new Rect(imageWidth, 0, imageWidth, imageHeight));
                drawingContext.DrawImage(frame3, new Rect(imageWidth * 2, 0, imageWidth, imageHeight));
            }

            // Converts the Visual (DrawingVisual) into a BitmapSource
            RenderTargetBitmap bmp = new RenderTargetBitmap(imageWidth * 3, imageHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            // Creates a PngBitmapEncoder and adds the BitmapSource to the frames of the encoder
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));

            // Saves the image into a file using the encoder
            string fileName = eachPanel.GetAuthor() + "_" + eachPanel.GetPosition() + ".jpg";
            string filePath = competitionDirectory + "/" + fileName;
            using (Stream stream = File.Create(filePath))
                encoder.Save(stream);
        }

        private static AbstractCompetition LoadSingleImageCompetition(string competitionFileName, string competitionDirectory, XmlNode rootNode, string clubName, string trophyName, bool scoring)
        {
            AbstractCompetition competition = new Competition(competitionFileName, competitionDirectory, clubName, trophyName);
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
            return competition;
        }
    }
}
