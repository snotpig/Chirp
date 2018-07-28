using System.Collections.Generic;
using System.Windows;

namespace Chirp
{
    /// <summary>
    /// Interaction logic for AddShowDlg.xaml
    /// </summary>
    public partial class AddShowDlg : Window
    {
        public IEnumerable<NewShow> NewShows { get; set; }

        public AddShowDlg()
        {
            InitializeComponent();
            dgNew.ItemsSource = NewShows;
        }

        public void SetNewShows(IEnumerable<NewShow> newShows)
        {
            NewShows = newShows;
            dgNew.ItemsSource = NewShows;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
