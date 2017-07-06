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

    public class ExtractDbPediaEntity
    {
        public static void Extract(string[] args)
        {
            const string URL = @"https://tagme.d4science.org/tagme/tag?include_categories=true&gcube-token=6aff7bdf-85d0-4141-8f26-f4c1576731d4-843339462&text={0}";
            //string inFile = @"D:\codeMix\QT Pairs\TriplesQuestionDataSetAsDBPedia.tsv";
            string inFile = args[1];
            string[] Lines = File.ReadAllLines(inFile);
            int count = 0;

            var invalidChars = Path.GetInvalidFileNameChars();
            var argsArray = args[2].Where(x => !invalidChars.Contains(x)).ToArray();
            string outFile = new string(argsArray);
            StreamWriter outsw = new StreamWriter(outFile);

            foreach (var line in Lines)
            {
                string[] cols = line.Split('\t');
                if (cols != null && cols.Count() == 2)
                {
                    string DataToTag = cols[0];
                    Console.WriteLine("Data to tag: " + DataToTag);
                    if (!String.IsNullOrEmpty(DataToTag))
                    {
                        try
                        {
                            WebRequest request = WebRequest.Create(string.Format(URL, DataToTag));
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
                                        StringBuilder sb = new StringBuilder();
                                        foreach (var annotation in responseDetail.annotations)
                                        {
                                            string title = annotation.title;
                                            if (!String.IsNullOrEmpty(title))
                                            {
                                                title = title.Replace(" ", "_");
                                                sb.Append(title + ";");
                                            }
                                        }
                                        Console.WriteLine("Tagged Values: " + sb.ToString());
                                        outsw.WriteLine(line + "\t" + sb.ToString());
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(count + "\t" + e.Message);
                                    outsw.WriteLine(line + "\t" + "NA");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(count + "\t" + e.Message);
                        }
                    }
                    else
                    {
                        outsw.WriteLine(line + "\t" + "NA");
                    }
                }
            }
            outsw.Close();
            Console.WriteLine("Done Writing to the file: " + outFile);
        }
    }
}