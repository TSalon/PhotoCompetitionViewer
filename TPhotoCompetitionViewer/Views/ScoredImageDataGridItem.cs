using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPhotoCompetitionViewer.Views
{
    class ScoredImageDataGridItem
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public short Score { get; set; }
        public object Timestamp { get; set; }
        public string ImagePath { get; set; }
        public bool Held { get; set; }
    }
}
