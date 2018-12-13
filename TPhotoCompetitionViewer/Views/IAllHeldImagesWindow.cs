using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPhotoCompetitionViewer.Competitions;

namespace TPhotoCompetitionViewer.Views
{
    interface IAllHeldImagesWindow
    {
        void MarkAwardedImages();

        void ShowSingleImageWindow(string competitionName, CompetitionImage image, SQLiteConnection dbConnection);
    }
}
