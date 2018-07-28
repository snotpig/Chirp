using log4net;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Chirp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Chirper _chirper;

        public MainWindow()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
            _chirper = new Chirper();
            txtFolder.Text = _chirper.Folder;
            datagrid.ItemsSource = _chirper.GetShows();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            btnGo.Visibility = Visibility.Collapsed;
            progress.Visibility = Visibility.Visible;

            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Go;
            worker.ProgressChanged += ProgressChanged;
            worker.RunWorkerCompleted += Finish;
            worker.RunWorkerAsync();
        }

        private void Go(object sender, DoWorkEventArgs e)
        {
            _chirper.Go(datagrid.Items.OfType<Show>(), (sender as BackgroundWorker));           
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progress.Value = e.ProgressPercentage;
        }

        private void Finish(object sender, RunWorkerCompletedEventArgs e)
        {
            btnGo.Visibility = Visibility.Visible;
            progress.Visibility = Visibility.Collapsed;
            datagrid.ItemsSource = _chirper.GetShows();
        }

        private void btnFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.SelectedPath = txtFolder.Text;
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFolder.Text = _chirper.Folder = folderDialog.SelectedPath;
                datagrid.ItemsSource = _chirper.GetShows();
            }              
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _chirper.SaveData();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            datagrid.ItemsSource = _chirper.GetShows().ToList();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addShowDlg = new AddShowDlg();
            addShowDlg.Owner = this;
            addShowDlg.dgNew.ItemsSource = _chirper.GetNewShows();
            var result = addShowDlg.ShowDialog();
            if (result.Value)
            {
                var newShows = addShowDlg.dgNew.Items.OfType<NewShow>().Where(s => s.Include);
                var existingShows = _chirper.GetShows().Select(s => s.ShowName).ToList();
                _chirper.AddNewShows(newShows.Select(s => s.Name).Distinct().Where(s => !existingShows.Contains(s)).ToDictionary(s => s, s => newShows.First(n => n.Name == s).ShortName));
                datagrid.ItemsSource = _chirper.GetShows().ToList();
            }
        }

        private void btnAddType_Click(object sender, RoutedEventArgs e)
        {
            var addTypeDlg = new EditTypeDlg();
            addTypeDlg.Owner = this;
            addTypeDlg.dgType.ItemsSource = _chirper.GetTypes().Select( t => new TypeItem { Type = t, Include = true }).ToList();
            var result = addTypeDlg.ShowDialog();
            if (result.Value)
            {
                _chirper.SetTypes(addTypeDlg.dgType.Items.OfType<TypeItem>().Where(i => i.Include).Select(i => i.Type).ToList());
                datagrid.ItemsSource = _chirper.GetShows().ToList();
            }
        }
    }

    class TypeItem
    {
        public string Type { get; set; }
        public bool Include { get; set; }
    }
}
