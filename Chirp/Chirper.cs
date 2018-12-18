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
        private IDictionary<string, string> _showNames;
        private HashSet<string> _fileTypes;

        public string Folder { get; set; }

        public Chirper()
        {
            LoadSavedData();
        }

        public ICollection<Show> GetShows()
        {
            var files = GetAudioFiles();

            var shows = files.Where(f => _fileTypes.Contains(f.Type) && _showNames.Keys.Any(k => f.ShowName.StartsWith(k))).Select(file =>
            {
                var parts = file.ShowName.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                var show = parts.First().Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                var fullTitle = string.Join("-", (parts.Where((w, i) => i > 0))).Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                var s = Array.IndexOf(show, "Series");
                var isDate = DateTime.TryParse(fullTitle.FirstOrDefault()?.Trim(new[] { '_' }), out var date);
                var isEpisode = int.TryParse(fullTitle.FirstOrDefault()?.Trim(new[] { '.' }), out var episode);
                var title = string.Join(" ", fullTitle.Where((w, i) => (s < 0 && !(isDate || isEpisode)) || i > 0));
                var showName = string.Join("_", show.Where((w, i) => s < 0 || i < s));

                return new Show
                {
                    FilePath = file.Path,
                    FileName = file.FileName,
                    ShowName = showName,
                    ShortName = _showNames[showName],
                    Title = title.StartsWith("Episode") ? "" : title,
                    Series = s > 0 && show.Length > s ? int.Parse(show.ElementAt(s + 1)) : 0,
                    Episode = isEpisode ? episode : 0,
                    Date = date == DateTime.MinValue ? new DateTime?() : date
                };
            });
            return shows.ToList();
        }

        public void Go(IEnumerable<Show> items, BackgroundWorker worker)
        {
            var total = 2 * items.Count();
            var i = 0;
            foreach (Show show in items)
            {
                var showName = show.ShowName.Replace(' ', '_');
                var episode = $"{(show.Episode > 0 ? "_" + show.Series.ToString() + "-" : "")}{(show.Episode > 0 ? show.Episode.ToString() : "")}";
                var date = show.Date.HasValue ? $"_{show.Date.Value.ToString("yyyyMMdd")}" : "";
                var title = string.IsNullOrWhiteSpace(show.Title) ? "" : $"_{show.Title.Replace(' ', '_')}";

                var file = TagLib.File.Create(show.FilePath, TagLib.ReadStyle.None);
                file.Tag.Title = $"{show.ShortName}{episode}{date}{title}";
                file.Save();
                i++;
                worker.ReportProgress(i * 100 / total);
                var newPath = show.FilePath.Replace(show.FileName, $"{showName}{episode}{date}{title}.m4a");
                File.Move(show.FilePath, newPath);
                i++;
                worker.ReportProgress(i * 100 / total);
            }
        }

        public ICollection<string> GetExistingShows()
        {
            return _showNames.Keys;
        }

        public ICollection<NewShow> GetNewShows()
        {
            var files = GetAudioFiles().Where(f => _fileTypes.Contains(f.Type));

            return files.Where(f => !_showNames.Keys.Any(k => f.ShowName.StartsWith(k))).Select(f =>
            {
                var name = f.ShowName.Split(new[] { "-", "Series" }, StringSplitOptions.RemoveEmptyEntries).First().TrimEnd(new[] { '_' });
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

        public void AddNewShows(IDictionary<string, string> newShows)
        {
            foreach (var show in newShows)
                _showNames.Add(show);
        }

        public void SaveData()
        {
            var serializer = new XmlSerializer(typeof(SaveObject));
            var saveObject = new SaveObject
            {
                Folder = Folder,
                FileTypes = new HashSet<string>(_fileTypes),
                Shows = _showNames.Keys.Select(k => new ShowNames { LongName = k, ShortName = _showNames[k] }).ToList()
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
                    _showNames = saveObject.Shows.ToDictionary(s => s.LongName, s => s.ShortName);
                }
            }
            else
            {
                Folder = @"C:\";
                _fileTypes = new HashSet<string> { "original" };
                _showNames = new Dictionary<string, string> { { "5_live_Science", "5LiveScience" } };
            }
        }

        private IEnumerable<M4aFile> GetAudioFiles()
        {
            return System.IO.Directory.EnumerateFiles(Folder, "*.m4a")
                .Select(file =>
                {
                    var fileName = Path.GetFileName(file);
                    var parts = fileName.Split(new[] { "_", ".m4a" }, StringSplitOptions.RemoveEmptyEntries);
                    return new M4aFile
                    {
                        Path = file,
                        FileName = fileName,
                        Type = parts.Last(),
                        ShowName = string.Join("_", parts.Where((string s, int i) => i < parts.Length - 2))
                    };
                });
        }
    }
}
