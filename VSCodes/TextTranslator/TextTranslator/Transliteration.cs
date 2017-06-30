using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.IndicInput.Client.Transliteration;
//using System.NaturalLanguage.Tools;

namespace TextTranslator
{
    class Transliteration
    {
        public static BasicTransliterationServer transliterationServer;
        public static Dictionary<string, int> LangTypeDictionary = new Dictionary<string, int>()
                                                                       {
                                                                           { "Bengali", 0x0445 },
                                                                           { "Gujarati", 0x0447 },
                                                                           { "Hindi", 0x0439 },
                                                                           { "Kannada", 0x044b },
                                                                           { "Malayalam", 0x044c },
                                                                           { "Marathi", 0x044e },
                                                                           { "Oriya", 0x0442 },
                                                                           { "Punjabi", 0x0446 },
                                                                           { "Tamil", 0x0449 },
                                                                           { "Telugu", 0x044a },
                                                                           { "Urdu", 0x441 },
                                                                       };
        public static void Transliterate(String[] args)
        {
            string language = args[1];
            string transliteratedFile = @"C:\CodeMixing\QT Pairs\AllCodeMix.tsv";
            transliterationServer = new BasicTransliterationServer();
            transliterationServer.Initialize(LangTypeDictionary[language], "Transliteration Sample");
            string[] transliterationLines = File.ReadAllLines(transliteratedFile);
            using (StreamWriter sw = System.IO.File.CreateText(@"C:\CodeMixing\QT Pairs\Transliteration.tsv"))
            {
                sw.WriteLine("RomanHindi\tIndicWord");
            }
            foreach (string line in transliterationLines)
            {
                string[] cols = line.Split('\t');
                if (cols != null && cols.Count() == 4)
                {
                    string[] words = cols[3].Split(' ');
                    StringBuilder sb = new StringBuilder();

                    if (words != null && words.Count() > 0)
                    {
                        foreach (string word in words)
                        {
                            string[] transliteratedStrings = transliterationServer.Transliterate(word, 2);
                            string transS = transliteratedStrings.Length > 0 ? transliteratedStrings[0] : String.Empty;
                            sb.Append(transS + " ");
                        }
                    }
                    string trans = sb.ToString();
                    using (StreamWriter sw = System.IO.File.AppendText(@"C:\CodeMixing\QT Pairs\Transliteration.tsv"))
                    {
                        sw.WriteLine(cols[3] + "\t" + trans);
                        Console.WriteLine("Transliterated:" + trans);
                    }
                }
            }
        }
    }
}


        