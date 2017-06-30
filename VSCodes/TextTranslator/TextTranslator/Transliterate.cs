using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextTranslator
{
    public class Transliterate
    {
        public TransliterationMapping translateObj;

        private static Transliterate _instance = null;
        private static readonly object _syncRoot = new object();

        public static Transliterate GetInstance()
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    _instance = _instance == null ? new Transliterate() : _instance;
                }
            }

            return _instance;
        }

        private Transliterate()
        {
            translateObj = new TransliterationMapping();
        }
        public String ToEnglish(String input, String lang)
        {
            if (lang.Equals(Constants.HindiLang))
            {
                input = translateObj.CheckWhitelist(input, Constants.SaavnHindiWhiteList);                              
                input = translateObj.CheckWhitelist(input, Constants.StarCastWhiteList);
                input = translateObj.CheckWhitelist(input, Constants.StopWordsWhiteList);
                input = translateObj.CheckWhitelist(input, Constants.JioMetaDataWhiteList);

                var chars = input.ToArray().Select(c => c.ToString()).ToArray();
                String mappedChar = String.Empty;
                StringBuilder englishOutput = new StringBuilder();
                if (chars.Length > 0)
                {
                    for (int ctr = 0; ctr < chars.Length; ctr++)
                    {
                        if (ctr == 0 && chars.Length == 1)
                            mappedChar = translateObj.GetTransliterateChar(chars[ctr], Constants.FIRST, Constants.LAST);
                        else if (ctr == 0)
                            mappedChar = translateObj.GetTransliterateChar(chars[ctr], Constants.FIRST, chars[ctr + 1]);
                        else if (ctr == chars.Length - 1)
                            mappedChar = translateObj.GetTransliterateChar(chars[ctr], chars[ctr - 1], Constants.LAST);
                        else
                            mappedChar = translateObj.GetTransliterateChar(chars[ctr], chars[ctr - 1], chars[ctr + 1]);
                        englishOutput.Append(mappedChar);
                    }
                }
                return englishOutput.ToString();
            }
            return input;
        }

        public Dictionary<String, String> ToEnglishList(String[] inputList, String lang)
        {
            Dictionary<String, String> outputList = new Dictionary<String, String>();
            foreach (String input in inputList)
            {
                if (!outputList.ContainsKey(input))
                    outputList.Add(input, ToEnglish(input, lang));
            }
            return outputList;
        }

    }
}
