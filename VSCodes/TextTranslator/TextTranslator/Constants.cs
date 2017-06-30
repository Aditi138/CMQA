using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TextTranslator
{
    public class Constants
    {
        public const string FIRST = "first";
        public const string LAST = "last";
        public const string TransliterationMappingFile = "TextTranslator.HindiToEnglishMapping.json";
//@"C:\CodeMixing\Transliterations\TransliterateHiToEnglish\HindiToEnglishMapping.json";
        public const string FullWhiteList = "TextTranslator.FullWhiteList.json";
//@"C:\CodeMixing\Transliterations\TransliterateHiToEnglish\FullWhiteList.json";
        //public const string TransliterationEnglishStorageContainerName = "TransliterationEnglishStorageContainerName";
        public const string TransliterationUpdateTime = "TransliterationUpdateTime";
        public const string HindiLang = "hi-IN";
        public const string HindiToEnglishMapping = "HindiToEnglishMapping";
        public const string ExtraHindiToEnglishMapping = "ExtraHindiToEnglishMapping";
        public const string JioMetaDataWhiteList = "JioMetaDataWhiteList";
        public const string SaavnHindiWhiteList = "SaavnHindiWhiteList";
        public const string StarCastWhiteList = "StarCastWhiteList";
        public const string StopWordsWhiteList = "StopWordsWhiteList";
        public const string DottedConstant = "़"; // dot below word
        public const string BigI = "ी";
        public const string DotOnTop = "ं";
        public const string EnglishTransliterationBetaContainerName = "english-transliteration-beta";
        public const string EnglishTransliterationProdContainerName = "english-transliteration-prod";

    }
    public enum CharTypeEnum
    {
        K_ACT_V,                       //KEY_ACTUAL_VOWEL
        K_APP_V,                       //KEY_APPEND_VOWEL
        K_DOU_APP_V,                //KEY_DOUBLE_APPEND_VOWEL
        K_SIM_APP_V,                //KEY_SIMPLE_APPEND_VOWEL
        K_CON,                          //KEY_CONSONANT
        K_CUS_CON,                   //KEY_CUSTOM_CONSONANT
        K_NUM,                             //KEY_NUMBER
        K_OTH,                              //KEY_OTHER
        K_NOT_SPECIFIED                       //KEY_NOT_SPECIFIED
    }
}
