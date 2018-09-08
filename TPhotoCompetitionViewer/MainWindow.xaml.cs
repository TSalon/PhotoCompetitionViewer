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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TPhotoCompetitionViewer.Competitions;

namespace TPhotoCompetitionViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CompetitionManager competitionMgr = new CompetitionManager();

        public MainWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            this.LoadCompetitions();

        }

        private void LoadCompetitions()
        {
            this.StatusLabel.Content = "Loading competitions...";
            List<string> competitionNames = this.competitionMgr.GetCompetitions();

            if (competitionNames.Count == 0)
            {
                this.StatusLabel.Content = "No competitions found";
            }
            else
            {
                this.StatusLabel.Content = "Select competition to run:";

                this.Competition1Button.Visibility = Visibility.Visible;
                this.Competition1Button.Content = competitionNames[0];
                if (competitionNames.Count > 1)
                {
                    this.Competition2Button.Visibility = Visibility.Visible;
                    this.Competition2Button.Content = competitionNames[1];
                }
                
            }
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Competition1Button_Click(object sender, RoutedEventArgs e)
        {
            CompetitionPage competitionPage = new CompetitionPage();
            competitionPage.Init(this.competitionMgr, 0);
            competitionPage.Show();
        }

        private void Competition2Button_Click(object sender, RoutedEventArgs e)
        {
            CompetitionPage competitionPage = new CompetitionPage();
            competitionPage.Init(this.competitionMgr, 1);
            competitionPage.Show();
        }
    }
}
