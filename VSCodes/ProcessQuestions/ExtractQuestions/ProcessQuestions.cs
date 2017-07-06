using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace ExtractQuestions
{
    class ProcessQuestions
    {
        static void ExtractDBForQuestionsMain(string[] args)
        {
            const string URL = @"https://tagme.d4science.org/tagme/tag?include_categories=true&gcube-token=6aff7bdf-85d0-4141-8f26-f4c1576731d4-843339462&text={0}";
            string inFile = @"F:\codeMix\Questions\QuestionsDataset_{0}.tsv";
            for (int i = 25; i < 27; i++)
            {
                string fileName = String.Format(inFile, i);
                string[] Lines = File.ReadAllLines(fileName);
                int count = 0;

                string categoryFile = String.Format(@"F:\codeMix\categories\{0}.tsv", i);
                StreamWriter catsw = new StreamWriter(categoryFile);
                Dictionary<string, string> categories = new Dictionary<string, string>();

                string outFile = String.Format(@"F:\codeMix\TagMeExtracted\{0}.tsv", i);
                StreamWriter outsw = new StreamWriter(outFile);
                Dictionary<string, string> wikiEntities = new Dictionary<string, string>();

                foreach (var line in Lines)
                {
                    try
                    {
                        WebRequest request = WebRequest.Create(string.Format(URL, line));
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        if (response != null)
                        {
                            Stream dataStream = response.GetResponseStream();
                            StreamReader reader = new StreamReader(dataStream);
                            string responseFromServer = reader.ReadToEnd();

                            reader.Close();
                            dataStream.Close();
                            response.Close();

                            JavaScriptSerializer json_serializer = new JavaScriptSerializer();

                            try
                            {
                                Tagme responseDetail = json_serializer.Deserialize<Tagme>(responseFromServer);
                                if (responseDetail != null && responseDetail.annotations != null && responseDetail.annotations.Count() > 0)
                                {
                                    foreach (var annotation in responseDetail.annotations)
                                    {
                                        string title = annotation.title;
                                        string[] Categories = annotation.dbpedia_categories;
                                        if (!String.IsNullOrEmpty(title))
                                        {
                                            string value = string.Empty;
                                            bool existing = wikiEntities.TryGetValue(title, out value);
                                            if (!existing)
                                            {
                                                wikiEntities[title] = "0;" + line;
                                            }
                                            else
                                            {
                                                value = wikiEntities[title];
                                                string[] info = value.Split(';');
                                                if (info != null && info.Count() == 2)
                                                {
                                                    int val = 0;
                                                    Int32.TryParse(info[0], out val);
                                                    val = val + 1;
                                                    wikiEntities[title] = val.ToString() + ";" + line;
                                                }
                                            }
                                        }
                                        foreach (var category in Categories)
                                        {
                                            string value = string.Empty;
                                            bool existing = categories.TryGetValue(category, out value);
                                            if (!existing)
                                            {
                                                categories[category] = "0;" + line;
                                            }
                                            else
                                            {
                                                value = categories[category];
                                                string[] info = value.Split(';');
                                                if (info != null && info.Count() == 2)
                                                {
                                                    int val = 0;
                                                    Int32.TryParse(info[0], out val);
                                                    val = val + 1;
                                                    categories[category] = val.ToString() + ";" + line;
                                                }
                                            }
                                        }

                                    }
                                }
                            }

                            catch (Exception e)
                            {
                                Console.WriteLine(count + "\t" + e.Message);
                            }
                            count = count + 1;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(count + "\t" + e.Message);
                    }
                }

                foreach (KeyValuePair<string, string> keyValue in wikiEntities)
                {
                    string[] info = keyValue.Value.Split(';');
                    if (info != null && info.Count() == 2)
                    {
                        outsw.WriteLine(keyValue.Key + '\t' + info[0] + '\t' + info[1]);
                        Console.WriteLine("Writig to WikiEntities file");
                    }

                }
                foreach (KeyValuePair<string, string> keyValue in categories)
                {
                    string[] info = keyValue.Value.Split(';');
                    if (info != null && info.Count() == 2)
                    {
                        catsw.WriteLine(keyValue.Key + '\t' + info[0] + '\t' + info[1]);
                        Console.WriteLine("Writig to categories file");
                    }
                }
                outsw.Close();
                catsw.Close();
            }
           
        }
    }
}
