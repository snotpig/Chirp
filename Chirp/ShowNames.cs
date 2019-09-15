using System;

namespace Chirp
{
    [Serializable]
    public class ShowData
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }
		public bool UseDate { get; set; }
		public Category Category { get; set; }
	}
}
