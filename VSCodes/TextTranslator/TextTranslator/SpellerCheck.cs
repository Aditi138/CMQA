using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace TextTranslator
{
    class SpellerCheck
    {
        public static void Speller(string[] args)
        {
            string inputPath = args[1];
            string outPath = args[2];
            string BaseURL = @"https://api.cognitive.microsoft.com/bing/v5.0/spellcheck?text={0}&mkt=en-us";
            StreamWriter outsw = new StreamWriter(outPath);
            
            string[] lines = File.ReadAllLines(inputPath);
            Console.WriteLine("Reading Input: " + inputPath);

            foreach(string line in lines)
            {
                string[] cols = line.Split('\t');
                if(cols!=null && cols.Count() > 0)
                {
                    string question = cols[0];
                    Console.WriteLine("Reading input question: " + question);

                    string correctedQuestion = ApplySpeller(BaseURL, question);
                    if(!String.IsNullOrEmpty(correctedQuestion))
                    {
                        outsw.WriteLine(correctedQuestion);
                    }
                }
            }
            Console.WriteLine("Done writing: " + outPath);
            outsw.Close();
            


        }

        private static string ApplySpeller(string BaseURL, string question)
        {
            if(String.IsNullOrEmpty(question))
            {
                Console.WriteLine("Question is Null or Empty");
                return string.Empty;
            }
            string URL = String.Format(BaseURL, question);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
            req.Method = "GET";
            req.Headers.Add("Ocp-Apim-Subscription-Key", "31bd6c0b5a1143aab3ba4864f56268ba");
            req.Headers.Add("Retry-After", "2");

            try
            {
                HttpWebResponse response;
                response = (HttpWebResponse)req.GetResponse();
                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                    response.Close();

                    JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                    ParseSpellerClass responseDetail = json_serializer.Deserialize<ParseSpellerClass>(responseFromServer);
                    string corrected = question;
                    if (validateResponse(responseDetail))
                    {
                        foreach (var entry in responseDetail.flaggedTokens)
                        {
                            if (entry.suggestions != null && entry.suggestions.Count() > 0 && !String.IsNullOrEmpty(entry.token))
                            {
                                var correctedSuggestion = entry.suggestions.Where(s => s.score > 0.7).FirstOrDefault();
                                if (!String.IsNullOrEmpty(correctedSuggestion.suggestion))
                                {
                                    corrected = corrected.Replace(entry.token, correctedSuggestion.suggestion);
                                }
                            }
                        }
                    }
                    Console.WriteLine("Corrected string: " + corrected);
                    return corrected;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error in request " + e.Message);
            }
            return string.Empty;
        }

        private static bool validateResponse(ParseSpellerClass response)
        {
            if(response==null || response.flaggedTokens==null || response.flaggedTokens.Count() == 0)
            {
                Console.WriteLine("Response object is invalid");
                return false;
            }
            return true;
        }
    }
}
