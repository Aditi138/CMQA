using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;
using Microsoft;
using System.Net.Http;
using System.Xml;

namespace TextTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            //Generating a accessToken
            string URL = @"https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
            string token = string.Empty;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);

                req.Method = "POST";
                req.ContentType = "application/json";
                req.Accept = "application/jwt";
                req.ContentLength = 0;
                req.Headers.Add("Ocp-Apim-Subscription-Key", "bff26f762ffd4dd9a54ef7dce3680707");

                WebResponse response = req.GetResponse();
                if (response != null)
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    token = reader.ReadToEnd();
                    // Console.WriteLine(responseFromServer);

                    reader.Close();
                    dataStream.Close();
                    response.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //Translating Text English to Hindi
            if (!String.IsNullOrEmpty(token))
            {
                string TranslatorURL = @"https://api.microsofttranslator.com/v2/Http.svc/TranslateArray";
                string[] lines = File.ReadAllLines(@"G:\TranslatedTriplesQuestionDataSet_Hindi.tsv");

                string outFile = @"G:\TranslatedFrom_Hindi.tsv";
                StreamWriter catsw = new StreamWriter(outFile);

                foreach (string line in lines)
                {
                    string[] cols = line.Split('\t');
                    if (cols != null && cols.Count() == 4)
                    {
                        string translatedText = string.Empty;
                        try
                        {
                            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(TranslatorURL);

                            req.Method = "POST";
                            req.ContentType = "text/xml; encoding='utf-8'";
                            req.Headers.Add("Authorization", "Bearer" + " " + token);

                            var requestBodyFormat = "<TranslateArrayRequest><AppId/><From>en</From><Options/><Texts><string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">{0}</string></Texts><To>hi</To></TranslateArrayRequest>";
                            string postdata = String.Format(requestBodyFormat, cols[3]);
                            byte[] bytes;
                            bytes = System.Text.Encoding.ASCII.GetBytes(postdata);
                            req.ContentLength = bytes.Length;

                            Stream newStream = req.GetRequestStream();
                            newStream.Write(bytes, 0, bytes.Length);
                            newStream.Close();

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

                                //Console.WriteLine(responseFromServer);
                                translatedText = XmlParse(responseFromServer);

                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error in Request:" + e.Message);
                        }

                        if (String.IsNullOrEmpty(translatedText))
                        {
                            translatedText = cols[3];
                        }
                        Console.WriteLine("Processed Request:" + line);
                        catsw.WriteLine(cols[0] + "\t" + cols[1] + "\t" + cols[2] + "\t" + translatedText);
                    }
                }
                catsw.Close();

            }

        }

        private static string XmlParse(string responseFromServer)
        {
            string value = string.Empty;
            if (!String.IsNullOrEmpty(responseFromServer))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseFromServer);

                XmlNodeList parentNode = xmlDoc.GetElementsByTagName("TranslatedText");
                foreach (XmlNode node in parentNode)
                {
                    value = node.InnerText;

                }
            }
            return value;
        }
    }

}
