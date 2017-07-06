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
            string[] lines = File.ReadAllLines(@"C:\CodeMixed\CNN\cnn-text-classification-tf\data\WrongPredictedTripletQuestion_All.tsv");
            int Total = 0;
            int fileNumber = 0;
            string file = @"C:\CodeMixed\CNN\cnn-text-classification-tf\data\NegativeFiles\WrongData_{0}.tsv";

            while (Total < lines.Length)
            {
                string fileName = String.Format(file, fileNumber);
                StreamWriter catsw = new StreamWriter(fileName);
                int count = 0;
                string[] tempLines = Populatelines(lines, Total);
                fileNumber = fileNumber + 1;
                if (tempLines != null)
                {
                    foreach (string line in tempLines)
                    {
                        //string newLine = ModfiedLine(line);
                        catsw.WriteLine(line);
                        count = count + 1;
                    }
                    Console.WriteLine("Done FileName :" + fileName + "Count : " + tempLines.Length);
                }
                Total = Total + count;
                catsw.Close();
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
