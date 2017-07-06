using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;
using System.Net;


namespace ExtractWIKILinks
{
    class Program
    {
        static void Main(string[] args)
        {
            string inFile = @"C:\Users\adchau\Downloads\wikianswers-paraphrases-1.0\wikianswers-paraphrases-1.0\Onlyquestions.txt";
            string[] Lines = File.ReadAllLines(inFile);

            foreach(var line in Lines)
            {
                string alteredLine = line.Replace("\t", string.Empty);
                WebRequest request = WebRequest.Create("https://tagme.d4science.org/tagme/tag?lang=en&include_abstract=true&include_categories=true&gcube-token=<your Service Authorization Token>&text=" + line);  

            }
            
        }
    }
}
