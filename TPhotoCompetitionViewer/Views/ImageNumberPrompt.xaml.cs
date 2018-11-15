using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TPhotoCompetitionViewer.Views
{
    /// <summary>
    /// Interaction logic for ImageNumberPrompt.xaml
    /// </summary>
    public partial class ImageNumberPrompt : Window
    {
        private string selectedNumber;

        public ImageNumberPrompt()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleKeys);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.selectedNumber = this.ImageNumber.Text;
            this.Close();
        }

        /** Handle a key on the keyboard being pushed */
        private void HandleKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        internal int GetSelectedNumber()
        {
            return Convert.ToInt32(this.selectedNumber);
        }
    }
}
