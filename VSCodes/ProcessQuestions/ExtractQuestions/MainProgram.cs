using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractQuestions
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            if(args==null || args.Count() !=3)
            {
                Console.WriteLine("Please provide arguments as TagMe <inputFile> <outputFile>");
            }
            else if(args[0]=="TagMe")
            {
                ExtractDbPediaEntity.Extract(args);
            }
        }
    }
}
