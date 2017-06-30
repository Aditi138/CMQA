using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTranslator
{
    class MainProgram
    {
        public static void Main(string[] args)
        {
            string data = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            if(args==null)
            {
                Console.WriteLine("Enter 1.Transliterate Hindi or 2. Translate");
                //return;
            }

            if(args.Count() ==2 && args[0] == "Transliterate")
            {
                Transliteration.Transliterate(args);
            }
            else if(args.Count() == 3 && args[0] == "Translate")
            {
                Translate.TranslateFunction(args);
            }

            else if (args[0] == "TransliterateBack")
            {
                String Name = Transliterate.GetInstance().ToEnglish("भारत की राजधानी कहां हैं?", "hi-IN");
            }
            else if(args[0] == "Speller")
            {
                SpellerCheck.Speller();
            }
        }
    }
}
