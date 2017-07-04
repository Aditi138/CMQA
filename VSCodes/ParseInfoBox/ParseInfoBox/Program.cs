using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ParseInfoBox
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = @"C:\CodeMixing\InfoBox\Sample.json";
            string outputFile = @"C:\CodeMixing\InfoBox\Triples_Question.tsv";
            string[] lines = File.ReadAllLines(inputFile);
            StreamWriter outsw = new StreamWriter(outputFile);

            foreach (string line in lines)
            {
                Dictionary<string, string> AttrValuePairs = new Dictionary<string, string>();
                string lineToBeParsed = line.Trim();
                string[] cols = lineToBeParsed.Split('\t');
                if (cols != null && cols.Count() == 2)
                {
                    GenerateAttrValuePairs(AttrValuePairs, cols);

                    foreach (KeyValuePair<string, string> KV in AttrValuePairs)
                    {
                        if (KV.Value.Contains("@@"))
                        {
                            string[] setOfValues = KV.Value.Split(new string[] { "@@" }, StringSplitOptions.None);
                            if (setOfValues != null && setOfValues.Count() > 0)
                            {
                                foreach (string value in setOfValues)
                                {
                                    outsw.WriteLine(cols[0] + "\t" + KV.Key + "\t" + value);
                                }
                            }
                        }
                        else
                        {
                            outsw.WriteLine(cols[0] + "\t" + KV.Key + "\t" + KV.Value);
                        }
                    }
                }

            }
            outsw.Close();
        }

        private static void GenerateAttrValuePairs(Dictionary<string, string> AttrValuePairs, string[] cols)
        {
            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(cols[1]);

            var jObj = (JObject)obj;
            foreach (JToken token in jObj.Children())
            {
                if (token is JProperty)
                {
                    var property = token as JProperty;
                    string Key = property.Name;
                    if (property.Value.Type == JTokenType.Array)
                    {
                        var items = (JArray)property.Value;
                        foreach (var item in items)
                        {
                            GetKeyValueFromJToken(AttrValuePairs, Key, item);
                        }
                    }
                    if (property.Value.Type == JTokenType.Object)
                    {
                        var items = (JObject)property.Value;
                        GetKeyValueFromJToken(AttrValuePairs, Key, items);
                    }

                }


            }
        }

        private static void GetKeyValueFromJToken(Dictionary<string, string> AttrValuePairs, string Key, JToken item)
        {
            string outValue="";
            foreach (KeyValuePair<string, JToken> KV in (JObject)item)
            {
                if (KV.Key.Contains("type"))
                {
                    var TypeValue = item["type"].Value<string>();
                    if (!String.IsNullOrEmpty(TypeValue) && TypeValue.Contains("text"))
                    {
                        var Value = item["value"].Value<string>();
                        if (!String.IsNullOrEmpty(Value))
                        {
                            bool existing = AttrValuePairs.TryGetValue(Key, out outValue);
                            if (existing)
                            {
                                Value = AttrValuePairs[Key] + "@@" + Value;
                                AttrValuePairs[Key] = Value;
                            }
                            else
                            {
                                AttrValuePairs.Add(Key, Value);
                            }
                        }
                    }
                    else if (!String.IsNullOrEmpty(TypeValue) && TypeValue.Contains("link"))
                    {
                        var Value = item["text"].Value<string>();
                        if (!String.IsNullOrEmpty(Value))
                        {
                            bool existing = AttrValuePairs.TryGetValue(Key, out outValue);
                            if (existing)
                            {
                                Value = AttrValuePairs[Key] + "@@" + Value;
                                AttrValuePairs[Key] = Value;
                            }
                            else
                            {
                                AttrValuePairs.Add(Key, Value);
                            }
                        }
                    }
                }

            }
        }
    }
}
