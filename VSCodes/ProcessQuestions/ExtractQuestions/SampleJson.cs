using System;

namespace ExtractQuestions
{
    public class Tagme
    {
        public DateTime timestamp { get; set; }
        public int time { get; set; }
        public string test { get; set; }
        public string api { get; set; }
        public Annotation[] annotations { get; set; }
        public string lang { get; set; }
    }

    public class Annotation
    {
        public int id { get; set; }
        public string title { get; set; }
        public string[] dbpedia_categories { get; set; }
        public int start { get; set; }
        public float link_probability { get; set; }
        public float rho { get; set; }
        public int end { get; set; }
        public string spot { get; set; }
    }
}


