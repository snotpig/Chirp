using external_drive_lib;
using external_drive_lib.interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Chirp
{
	public class Chirper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        private IDictionary<string, ShowData> _showData;
        private HashSet<string> _fileTypes;

        public string Folder { get; set; }

        public Chirper()
        {
            LoadSavedData();
        }

        public ICollection<Show> GetShows()
        {
            var files = GetAudioFiles();

            var shows = files.Where(f => _fileTypes.Contains(f.Type) && _showData.Keys.Any(k => GetShowName(f.ShowFullName) == k)).Select(file =>
            {
                var parts = file.ShowFullName.Split(new[] { "_-_" }, StringSplitOptions.RemoveEmptyEntries);
                var show = parts.First().Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                var fullTitle = string.Join("-", (parts.Where((w, i) => i > 0))).Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                var s = Array.IndexOf(show, "Series");
                var isDate = DateTime.TryParse(fullTitle.FirstOrDefault()?.Trim(new[] { '_' }), out var date);
                var isEpisode = int.TryParse(fullTitle.FirstOrDefault()?.Trim(new[] { '.' }), out var episode);
                var title = string.Join(" ", fullTitle.Where((w, i) => !(isDate || isEpisode) || i > 0));
                var showName = string.Join("_", show.Where((w, i) => s < 0 || i < s));

                return new Show
                {
                    FilePath = file.Path,
                    FileName = file.FileName,
                    ShowName = showName,
                    ShortName = _showData[showName].ShortName,
                    Title = title.StartsWith("Episode") ? "" : title,
                    Series = s > 0 && show.Length > s ? int.Parse(show.ElementAt(s + 1)) : 0,
                    Episode = isEpisode ? episode : 0,
                    Date = date == DateTime.MinValue ? new DateTime?() : date,
					UseDate = _showData[showName].UseDate,
					Category = _showData[showName].Category
				};
            });
            return shows.ToList();
        }

        public string Go(IEnumerable<Show> items, BackgroundWorker worker)
        {
			SyncFolders(it => worker.ReportProgress(it));
			var total = items.Count();
			var i = 0;
			foreach (Show show in items)
			{
				var showName = show.ShowName.Replace(' ', '_');
				var episode = $"{(show.Series > 0 && show.Episode > 0 ? $"_{show.Series}-" : "")}{(show.Episode > 0 ? show.Episode.ToString() : "")}";
				var date = show.Date.HasValue ? $"_{show.Date.Value.ToString("yyyyMMdd")}" : "";
				var title = string.IsNullOrWhiteSpace(show.Title) ? "" : $"_{show.Title.Replace(' ', '_')}";

				var file = TagLib.File.Create(show.FilePath, TagLib.ReadStyle.None);
				file.Tag.Title = $"{show.ShortName}{episode}{date}{title}";
				file.Save();

				MoveFile(show.FilePath, $"{showName}{episode}{date}{title}.m4a", show.Category);
				i++;
				worker.ReportProgress(i * 100 / total);
			}
			return "Success";
        }

        public ICollection<string> GetExistingShows()
        {
            return _showData.Keys;
        }

        public ICollection<NewShow> GetNewShows()
        {
            var files = GetAudioFiles().Where(f => _fileTypes.Contains(f.Type));

            return files.Where(f => !_showData.Keys.Any(k => GetShowName(f.ShowFullName) == k)).Select(f =>
            {
                var name = GetShowName(f.ShowFullName);
                return new NewShow
                {
                    Name = name,
                    ShortName = name.Replace("_", ""),
                    Type = f.Type,
                    Include = true
                };
            }).GroupBy(f => f.Name).Select(g => g.First()).ToList();
        }

        public ICollection<string> GetTypes()
        {
            return _fileTypes;
        }

        public void SetTypes(ICollection<string> types)
        {
            _fileTypes = new HashSet<string>(types);
        }

        public void AddNewShows(IDictionary<string, ShowData> newShows)
        {
            foreach (var show in newShows)
                _showData.Add(show);
			SaveData();
        }

        public void SaveData()
        {
            var serializer = new XmlSerializer(typeof(SaveObject));
            var saveObject = new SaveObject
            {
                Folder = Folder,
                FileTypes = new HashSet<string>(_fileTypes),
                Shows = _showData.Keys.Select(k => new ShowData { LongName = k, ShortName = _showData[k].ShortName, UseDate = _showData[k].UseDate, Category = _showData[k].Category }).ToList()
            };
            if (!Directory.Exists(@"C:\ProgramData\Snotsoft"))
                Directory.CreateDirectory(@"C:\ProgramData\Snotsoft");
            if (!Directory.Exists(@"C:\ProgramData\Snotsoft\Chirp"))
                Directory.CreateDirectory(@"C:\ProgramData\Snotsoft\Chirp");

            using (var stream = File.Open(@"C:\ProgramData\Snotsoft\Chirp\SavedData.xml", FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, saveObject);
            }
        }

        private void LoadSavedData()
        {
            if (File.Exists(@"C:\ProgramData\Snotsoft\Chirp\SavedData.xml"))
            {
                var serializer = new XmlSerializer(typeof(SaveObject));
                using (var stream = File.OpenRead(@"C:\ProgramData\Snotsoft\Chirp\SavedData.xml"))
                {
                    var saveObject = (SaveObject)serializer.Deserialize(stream);
                    Folder = saveObject.Folder;
                    _fileTypes = saveObject.FileTypes;
                    _showData = saveObject.Shows.ToDictionary(s => s.LongName, s => new ShowData { ShortName = s.ShortName, UseDate = s.UseDate, Category = s.Category });
                }
            }
            else
            {
                Folder = @"C:\";
                _fileTypes = new HashSet<string> { "original" };
                _showData = new Dictionary<string, ShowData> { { "5_live_Science", new ShowData { ShortName = "5LiveScience", Category = Category.Science } } };
            }
        }

        private IEnumerable<M4aFile> GetAudioFiles()
        {
            return Directory.EnumerateFiles(Folder, "*.m4a")
                .Select(file =>
                {
                    var fileName = Path.GetFileName(file);
                    var parts = fileName.Split(new[] { "_", ".m4a" }, StringSplitOptions.RemoveEmptyEntries);
                    return new M4aFile
                    {
                        Path = file,
                        FileName = fileName,
                        Type = parts.Last(),
                        ShowFullName = string.Join("_", parts.Where((string s, int i) => i < parts.Length - 2 && (i < parts.Length - 3 || s != "-") ))
                    };
                });
        }

		private string GetShowName(string showFullName)
		{
			return showFullName
				.Split(new[] { "_-_", "Series" }, StringSplitOptions.RemoveEmptyEntries).First().Trim(new[] { '_' });
		}

		private void MoveFile(string filePath, string newFilename, Category category)
		{
			var folderPath = $@"{Folder}\{category}";
			File.Move(filePath, $@"{folderPath}\{newFilename}");

			var folder = drive_root.inst.parse_folder(folderPath);
			var file = folder.files.FirstOrDefault(f => f.full_path == $@"{folderPath}\{newFilename}");
			var audio = drive_root.inst.parse_folder(@"{354d10a67cf3}:\SanDisk SD card\Audio");
			if (file == null || audio == null)
				throw new IOException($"failed to copy {newFilename} to {folderPath}");

			file.copy_sync($@"{audio.full_path}/{category}");
		}

		public ConnectionState CheckPhoneConnectionState()
		{
			var phone = drive_root.inst.drives.Where(d => d.type.is_portable())
				.FirstOrDefault(d => d.friendly_name == "Redmi 3S");

			if (phone == null)
				return ConnectionState.NotConnected;

			try
			{
				var sdCard = drive_root.inst.parse_folder(@"{354d10a67cf3}:\SanDisk SD card");
			}
			catch(Exception)
			{
				return ConnectionState.Connected;
			}

			return ConnectionState.ConnectedMtp;
		}

		private void SyncFolders(Action<int> reportProgress)
		{
			var categories = Enum.GetNames(typeof(Category)).ToList();
			var pcFiles = categories.SelectMany(c => Directory.Exists($@"{Folder}\{c}")
				? Directory.EnumerateFiles($@"{Folder}\{c}", "*.m4a").Select(f => new AudioFile(f, c))
				: new List<AudioFile>())
				.ToLookup(f => f.Category);

			var phoneFiles = drive_root.inst.parse_folder($@"{{354d10a67cf3}}:\SanDisk SD card\Audio").child_folders
				.Where(f => categories.Contains(f.name)).Select(f => f.name)
				.SelectMany(c => drive_root.inst.parse_folder($@"{{354d10a67cf3}}:\SanDisk SD card\Audio\{c}").files
				.Select(f => new AudioFile(f.full_path, c)))
				.ToLookup(f => f.Category);

			var total = phoneFiles.Count();
			var mf = 50 / total;
			foreach (var file in pcFiles.SelectMany(c => c.Where(f => !phoneFiles[c.Key].Select(af => af.FileName).Contains(f.FileName))))
			{
				if (File.GetCreationTime(file.FilePath) < DateTime.Now.Subtract(new TimeSpan(14, 0, 0, 0)))
				{
					MoveFileToRecycleBin(file.FilePath);
					reportProgress(50 - mf * total--);
				}
			}
		}

		private void MoveFileToRecycleBin(string filePath)
		{
			Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(filePath, 
				Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, 
				Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
		}
	}

	public class AudioFile
	{
		public string FilePath { get; set; }
		public string FileName { get { return Path.GetFileName(FilePath); } }
		public string Category { get; set; }

		public AudioFile(string filePath, string category)
		{
			FilePath = filePath;
			Category = category;
		}
	}
}
