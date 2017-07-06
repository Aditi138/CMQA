using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ExtractQuestions
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    string FileName = @"C:\Users\adchau\Downloads\wikianswers-paraphrases-1.0\wikianswers-paraphrases-1.0\questions.txt";
        //    string outFile = @"C:\Users\adchau\Downloads\wikianswers-paraphrases-1.0\wikianswers-paraphrases-1.0\Onlyquestions.txt";
        //    string[] Lines = File.ReadAllLines(FileName);
        //    string[] outputLines = new string[2600000];
        //    int count =0;
        //    foreach(var line in Lines)
        //    {
        //        string alteredline = line.Replace("'",string.Empty);
        //        string[] data = alteredline.Split('?');
        //        if(data!=null && data.Count() > 0)
        //        {
        //            Console.WriteLine("Read the line " + count);
        //            if (!String.IsNullOrEmpty(data[0]))
        //            {
        //                outputLines[count] = data[0]+ "?";
        //                count = count + 1;
        //            }
        //        }
                
        //    }

        //    File.WriteAllLines(outFile, outputLines);
        //    Console.WriteLine("Done");
        //}
    }
}
