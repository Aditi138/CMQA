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
        public static void Speller()
        {
            //string FilePath = @"C:\CodeMixing\SpellerInput.tsv";
            //string outFilePath = @"C:\CodeMixing\SpellerOutput.tsv";

            string BaseURL = @"https://api.cognitive.microsoft.com/bing/v5.0/spellcheck?text={0}&mkt=en-us";

            string text = "hello why are yuo yuo fdiffernt?";
            string URL = String.Format(BaseURL, text);

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
                    string corrected = text;
                    if(validateResponse(responseDetail))
                    {
                        foreach(var entry in responseDetail.flaggedTokens)
                        {
                            if(entry.suggestions!=null && entry.suggestions.Count() > 0 && !String.IsNullOrEmpty(entry.token))
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
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("error in request " + e.Message);
            }


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
