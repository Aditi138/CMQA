using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ParseInfoBox
{
    public class InfoBox
    {
        public Name name { get; set; }
        public Image image { get; set; }

        public Dictionary<String, Dictionary<String,String>> result;
    }

    public class Name
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Image
    {
        public string type { get; set; }
        public string text { get; set; }
        public string url { get; set; }
    }
}

