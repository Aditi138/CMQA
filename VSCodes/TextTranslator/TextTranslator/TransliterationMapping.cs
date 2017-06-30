//using CommonLibrary.AzureStorage;
using System;
using System.Collections.Generic;
using System.IO;
//using Microsoft.WindowsAzure.Storage.Blob;
//using static CommonLibrary.AzureStorage.ClassifierAzureBlobManager;
using Newtonsoft.Json;
//using Microsoft.Azure;
using System.Reflection;
using System.Text;

namespace TextTranslator
{
    public class TransliterationMapping
    {
        static Dictionary<String, Symbol> transliterateMap = new Dictionary<string, Symbol>();
        static Dictionary<String, Symbol> specialMap = new Dictionary<string, Symbol>();
        static Dictionary<String, String> JioMetaDataWhiteList = new Dictionary<String, String>();
        static Dictionary<String, String> SaavnHindiWhiteList = new Dictionary<String, String>();
        static Dictionary<String, String> StarCastWhiteList = new Dictionary<String, String>();
        static Dictionary<String, String> StopWordsWhiteList = new Dictionary<String, String>();
        private static bool isTransliterationUpdating = false;
        private long transliterationVersion;
        private long transliterationUpdateTime = 0;
        public TransliterationMapping()
        {
    
            BuildTransliterateMapping();
        }

        public void BuildTransliterateMapping()
        {
            

                  var transliterationFiles = new List<string>();
                  transliterationFiles.Add(Constants.TransliterationMappingFile);
                  transliterationFiles.Add(Constants.FullWhiteList);
                  LoadTransliterateMappingFromLocalFile(transliterationFiles);

        }


        public void LoadTransliterateMappingFromLocalFile(List<string> files)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            foreach (var file in files)
            {
                using (Stream stream = assembly.GetManifestResourceStream(file))
                {
                    //StreamReader stream = new StreamReader(file);
                    LoadDictionaries(stream);
                }
            }
        }

        public static void AddToWhiteList(ListObject list, Dictionary<String,String> whitelist)
        {
            foreach (var obj in list.Dictionary)
            {
                for (int i = 1; i < obj.Count; i++)
                {
                    if(!whitelist.ContainsKey(obj[i]))
                    whitelist.Add(obj[i], obj[0]);
                }
            }
        }

        public static void AddToMapping(ListObject list, Dictionary<string, Symbol> map)
        {
            foreach (var obj in list.Dictionary)
            {
                map.Add(obj[0], new Symbol(obj[1], (CharTypeEnum)Enum.Parse(typeof(CharTypeEnum), obj[2])));
            }

        }

        public static void LoadDictionaries(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                var jsonStr = reader.ReadToEnd().Normalize(System.Text.NormalizationForm.FormKD);
                var json = Newtonsoft.Json.JsonConvert.DeserializeObject<TransliterateJsonObject>(jsonStr);
                
                foreach (var list in json.FullList)
                {
                    switch (list.Type)
                    {
                        case Constants.HindiToEnglishMapping:

                            AddToMapping(list, transliterateMap);
                            break;

                        case Constants.ExtraHindiToEnglishMapping:

                            AddToMapping(list, specialMap);
                            break;

                        case Constants.JioMetaDataWhiteList:

                            AddToWhiteList(list, JioMetaDataWhiteList);
                            break;

                        case Constants.SaavnHindiWhiteList:

                            AddToWhiteList(list, SaavnHindiWhiteList);
                            break;

                        case Constants.StarCastWhiteList:

                            AddToWhiteList(list, StarCastWhiteList);
                            break;

                        case Constants.StopWordsWhiteList:

                            AddToWhiteList(list, StopWordsWhiteList);
                            break;
                    }
                }
            }
        }
        public Boolean SanityTest()
        {
            return transliterateMap["?"].symbol.Equals("k") && specialMap["?"].symbol.Equals("q") && JioMetaDataWhiteList["??"].Equals("the") && SaavnHindiWhiteList["??"].Equals("hai") && StarCastWhiteList["???"].Equals("khan") && StopWordsWhiteList["???"].Equals("call");
        }
        public String GetTransliterateChar(String current, String before, String after)
        {

            if (transliterateMap.ContainsKey(current))
            {
                // special character small bindi below words
                if (before.Equals(Constants.FIRST))
                {
                    if (after.Equals(Constants.DottedConstant) && specialMap.ContainsKey(current))
                        return specialMap[current].symbol;
                    else
                        return transliterateMap[current].symbol;
                }
                else if (after.Equals(Constants.DottedConstant) && specialMap.ContainsKey(current))
                {
                    if (transliterateMap.ContainsKey(before))
                    {
                        if (transliterateMap[before].key.ToString().Contains("CON"))
                            return "a" + specialMap[current].symbol;
                    }
                    return specialMap[current].symbol;
                }
                else if (current.Equals(Constants.DottedConstant) && transliterateMap.ContainsKey(after))
                {
                    if (!transliterateMap[after].key.ToString().Contains("CON"))
                        return "";
                }

                // special matra badi ee
                else if (current.Equals(Constants.BigI) && transliterateMap.ContainsKey(before))
                {
                    if ((after.Equals(Constants.LAST) || after.Equals(" ") || !transliterateMap.ContainsKey(after)))
                        return "i";
                    else
                        return transliterateMap[current].symbol;
                }

                else if (transliterateMap[current].key.ToString().Contains("CON") && transliterateMap.ContainsKey(before))
                {
                    if (transliterateMap[before].key.ToString().Contains("CON"))
                        return "a" + transliterateMap[current].symbol;
                    else
                        return transliterateMap[current].symbol;

                }
                else if (current.Equals(Constants.DotOnTop) && transliterateMap.ContainsKey(before))
                {
                    if (transliterateMap[before].key.ToString().Contains("CON"))
                        return "an";
                    else
                        return transliterateMap[current].symbol;

                }
                else
                    return transliterateMap[current].symbol;
            }
            return current;
        }

        //Check WhiteList
        public string CheckWhitelist(String input, String whiteList)
        {
            if (whiteList.Equals(Constants.JioMetaDataWhiteList))
                return CheckWhiteList(input, JioMetaDataWhiteList);

            else if (whiteList.Equals(Constants.SaavnHindiWhiteList))
                    return CheckWhiteList(input, SaavnHindiWhiteList);

            else if (whiteList.Equals(Constants.StarCastWhiteList))
                return CheckWhiteList(input, StarCastWhiteList);
            
            else if (whiteList.Equals(Constants.StopWordsWhiteList))
                return CheckWhiteList(input, StopWordsWhiteList);

            else return input;

        }

        public string CheckWhiteList(String input, Dictionary<String,String> whitelist)
        {
            String[] wordsList = input.Split(' ');
            foreach (String word in wordsList)
            {
                if (whitelist.ContainsKey(word))
                    input = input.Replace(word, whitelist[word]);
            }
            return input;
        }       

    }
}