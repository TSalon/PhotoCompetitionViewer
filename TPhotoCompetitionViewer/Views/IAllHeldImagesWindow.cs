using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPhotoCompetitionViewer.Views
{
    interface IAllHeldImagesWindow
    {
        void MarkAwardedImages();

        void ShowSingleImageWindow(string competitionName, string imagePath, SQLiteConnection dbConnection);
    }
}
