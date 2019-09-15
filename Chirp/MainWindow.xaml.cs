using log4net;
using System.Collections.ObjectModel;
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
		private ObservableCollection<Show> _shows;

		public MainWindow()
		{
			InitializeComponent();
			log4net.Config.XmlConfigurator.Configure();
			_chirper = new Chirper();
			txtFolder.Text = _chirper.Folder;
			GetShows();
		}

		private void GetShows()
		{
			_shows = new ObservableCollection<Show>(_chirper.GetShows());
			datagrid.ItemsSource = _shows;
			progress.Visibility = Visibility.Collapsed;
			btnGo.Visibility = Visibility.Visible;
			btnGo.IsEnabled = _shows.Count > 0;
		}

		private void btnGo_Click(object sender, RoutedEventArgs e)
		{
			if (_shows.All(i => i.Date.HasValue || !i.UseDate))
			{
				progress.Value = 0;
				btnGo.Visibility = Visibility.Collapsed;
				progress.Visibility = Visibility.Visible;

				var worker = new BackgroundWorker
				{
					WorkerReportsProgress = true
				};
				worker.DoWork += Go;
				worker.ProgressChanged += ProgressChanged;
				worker.RunWorkerCompleted += Finish;
				worker.RunWorkerAsync();
			}
		}

		private void Go(object sender, DoWorkEventArgs e)
		{
			e.Result = _chirper.Go(datagrid.Items.OfType<Show>(), (sender as BackgroundWorker));
		}

		private void ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progress.Value = e.ProgressPercentage;
			//datagrid.ItemsSource = _chirper.GetShows();
		}

		private void Finish(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
				MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			else if (e.Result is string result && result != "Success")
				MessageBox.Show(result, "Connection problem", MessageBoxButton.OK, MessageBoxImage.Warning);
			GetShows();
		}

		private void btnFolder_Click(object sender, RoutedEventArgs e)
		{
			var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
			folderDialog.SelectedPath = txtFolder.Text;
			if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				txtFolder.Text = _chirper.Folder = folderDialog.SelectedPath;
				GetShows();
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_chirper.SaveData();
		}

		private void btnRefresh_Click(object sender, RoutedEventArgs e)
		{
			GetShows();
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
				_chirper.AddNewShows(newShows.Select(s => s.Name).Distinct()
					.Where(s => !existingShows.Contains(s)).ToDictionary(s => s, s =>
					{
						var newShow = newShows.First(n => n.Name == s);
						return new ShowData { LongName = newShow.Name, ShortName = newShow.ShortName, UseDate = newShow.UseDate, Category = newShow.Category };
					}));
				GetShows();
			}
		}

		private void btnAddType_Click(object sender, RoutedEventArgs e)
		{
			var addTypeDlg = new EditTypeDlg();
			addTypeDlg.Owner = this;
			addTypeDlg.dgType.ItemsSource = _chirper.GetTypes().Select(t => new TypeItem { Type = t, Include = true }).ToList();
			var result = addTypeDlg.ShowDialog();
			if (result.Value)
			{
				_chirper.SetTypes(addTypeDlg.dgType.Items.OfType<TypeItem>().Where(i => i.Include).Select(i => i.Type).ToList());
				GetShows();
			}
		}
	}

	class TypeItem
    {
        public string Type { get; set; }
        public bool Include { get; set; }
    }
}
