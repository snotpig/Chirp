using System;

namespace Chirp
{
	public class Show
    {
        public string ShowName { get; set; }
        public string ShortName { get; set; }
        public int Series { get; set; }
        public int Episode { get; set; }
        public DateTime? Date { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
		public bool UseDate { get; set; }
		public Category Category { get; set; }
        public double Size { get; set; }
    }
}
