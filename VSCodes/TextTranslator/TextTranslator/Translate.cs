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
using System.Threading;

namespace TextTranslator
{
    public class Translate
    {
        public static void TranslateFunction(string[] args)
        {
            if (args == null || args.Count() != 3)
            {
                Console.WriteLine("Enter Translate <from> <to>");
                return;
            }

            string from = args[1];
            string to = args[2];

            //Generating a accessToken
            string token = GenerateAccessToken();

            //Translating Text English to Hindi
            string FilePath = @"C:\CodeMixing\TransliterateInput2.tsv";
            string outFilePath = @"C:\CodeMixing\Transliterations\AllCodeMixedTranslate_{0}.tsv";
            if (!String.IsNullOrEmpty(token))
            {

                for (int i = 1; i <= 1; i++)
                {
                    string TranslatorURL = @"https://api.microsofttranslator.com/v2/Http.svc/Translate?text={0}&from={1}&to={2}";

                    Console.WriteLine("Starting file: " + i);
                    string inFile = String.Format(FilePath, i);
                    string outFile = String.Format(outFilePath, i);
                    string[] lines = File.ReadAllLines(inFile);

                    //string outFile = @"G:\TranslatedFrom_Eng_Hindi.tsv";
                    StreamWriter catsw = new StreamWriter(outFile);

                    foreach (string line in lines)
                    {
                        string[] cols = line.Split('\t');
                        if (cols != null && cols.Count() == 8)
                        {
                            string translatedText = string.Empty;
                            try
                            {
                                string toTranslate = cols[4];
                                if (!String.IsNullOrEmpty(toTranslate) && toTranslate.Contains("@"))
                                {
                                    Console.WriteLine("Translating: " + toTranslate);
                                    string[] words = toTranslate.Split(' ');
                                    if (words != null && words.Count() > 0)
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        foreach (string word in words)
                                        {
                                            if (word.Contains("@"))
                                            {
                                                string text = word.Replace("@", string.Empty);
                                                string URL = String.Format(TranslatorURL, text, from, to);
                                                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
                                                //string Url = String.Format(TranslatorURL, "क्या दक्षिण डेकोटा में landforms पाया", "hi", "en");
                                                //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);

                                                req.Method = "GET";
                                                req.ContentType = "text/xml; encoding='utf-8'";
                                                req.Headers.Add("Authorization", "Bearer" + " " + token);

                                                ////  var requestBodyFormat = "<TranslateArrayRequest><AppId/><From>en</From><Options/><Texts><string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">{0}</string></Texts><To>hi</To></TranslateArrayRequest>";
                                                // // string postdata = String.Format(requestBodyFormat, cols[3]);
                                                // // byte[] bytes;
                                                // // bytes = System.Text.Encoding.ASCII.GetBytes(postdata);
                                                // // req.ContentLength = bytes.Length;

                                                //  Stream newStream = req.GetRequestStream();
                                                //  newStream.Write(bytes, 0, bytes.Length);
                                                //  newStream.Close();

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
                                                    if (!String.IsNullOrEmpty(translatedText))
                                                    {
                                                        sb.Append(translatedText + " ");
                                                    }
                                                    else
                                                    {
                                                        sb.Append(text + " ");
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                sb.Append(word + " ");
                                            }
                                        }
                                        Console.WriteLine("Translated text: " + sb.ToString());
                                        catsw.WriteLine(cols[0] + "\t" + cols[1] + "\t" + cols[2] + "\t" + cols[3] + "\t" + sb.ToString() + "\t" + cols[5] + "\t" + "1" + "\t" + cols[7]);

                                    }

                                }
                                else
                                {
                                    catsw.WriteLine(cols[0] + "\t" + cols[1] + "\t" + cols[2] + "\t" + cols[3] + "\t" + cols[4] + "\t" + cols[5] + "\t" + "1" + "\t" + cols[7]);
                                }
                            }
                            catch (Exception e)
                            {
                                catsw.WriteLine(cols[0] + "\t" + cols[1] + "\t" + cols[2] + "\t" + cols[3] + "\t" + cols[4] + "\t" + cols[5] + "\t" + "0" + "\t" + cols[7]);
                                Console.WriteLine("Error in Request:" + e.Message);
                            }
                        }
                    }
                    catsw.Close();
                    Console.WriteLine("Done with file : " + i);
                    token = GenerateAccessToken();
                    Thread.Sleep(5000);
                }

            }

        }

        private static string GenerateAccessToken()
        {
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
            return token;
        }

        private static string XmlParse(string responseFromServer)
        {
            string value = string.Empty;
            if (!String.IsNullOrEmpty(responseFromServer))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseFromServer);

                XmlNodeList parentNode = xmlDoc.GetElementsByTagName("string");
                foreach (XmlNode node in parentNode)
                {
                    value = node.InnerText;

                }
            }
            return value;
        }
    }

}