using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SplitData
{
    class Program
    {
        static void Main(string[] args)
        {
            //string[] lines = File.ReadAllLines(@"C:\CodeMixed\CNN\cnn-text-classification-tf\data\WrongPredictedTripletQuestion_All.tsv");
            int Total = 0;
            int fileNumber = 0;
            string file = @"C:\CodeMixed\CNN\cnn-text-classification-tf\data\NegativeFiles\WrongData_{0}.tsv";

            //while (Total < lines.Length)
            int count = 0;
            string fileName = String.Format(file, fileNumber);
            StreamWriter catsw = new StreamWriter(fileName);
                    
            using (StreamReader reader = new StreamReader(@"C:\CodeMixed\CNN\cnn-text-classification-tf\data\WrongPredictedTripletQuestion_All.tsv"))
            {
                string line = "";
                
                while ((line = reader.ReadLine()) != null)
                {
                    if(count < 10000)
                    {
                        catsw.WriteLine(line);
                        count = count + 1;
                    }
                    else
                    {
                        Console.WriteLine("Done FileName :" + fileName + "Count : " + count);
                        count = 0;
                        fileNumber = fileNumber + 1;
                        fileName = String.Format(file, fileNumber);
                        catsw.Close();
                        catsw = new StreamWriter(fileName);
                        
                    }
                }
            }
        }

        private static string ModfiedLine(string line)
        {
            string newline = "";
            if(!String.IsNullOrEmpty(line))
            {
                string[] cols = line.Split('\t');
                if(cols!=null && cols.Count() > 0)
                {
                    newline = cols[0]+"\t"+cols[2];
                }
            }
            return newline;
        }

        private static string[] Populatelines(string[] lines, int Total)
        {
            string[] TempLines = new string[10000];
            if (lines != null && Total >= 0)
            {
                for (int i = 0; i < 10000;  i++)
                {
                    if (Total < lines.Length)
                    {
                        TempLines[i] = lines[Total];
                        Total = Total + 1;
                    }
                }
            }
            return TempLines;
        }
    }
}
