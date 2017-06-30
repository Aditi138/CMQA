using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTranslator
{
    class TransliterateJsonObject
    {
        [JsonProperty(PropertyName = "FullList")]
        public List<ListObject> FullList { get; set; }

    }
    public class ListObject
    {
        [JsonProperty(PropertyName = "Type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "KeyLang")]
        public string KeyLang { get; set; }

        [JsonProperty(PropertyName = "ValueLang")]
        public string ValueLang { get; set; }

        [JsonProperty(PropertyName = "Dictionary")]
        public List<List<string>> Dictionary { get; set; }
    }
}