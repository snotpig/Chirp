using System.Windows;

namespace Chirp
{
    /// <summary>
    /// Interaction logic for AddType.xaml
    /// </summary>
    public partial class EditTypeDlg : Window
    {
        public EditTypeDlg()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
