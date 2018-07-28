using System;
using System.Collections.Generic;

namespace Chirp
{
    [Serializable]
    public class SaveObject
    {
        public string Folder { get; set; }
        public HashSet<string> FileTypes { get; set; }
        public List<ShowNames> Shows { get; set; }
    }
}
