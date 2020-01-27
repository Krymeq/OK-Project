using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OK_Project
{
    class Program
    {
        /*
         * TO DO:
         * * check when we need to recalculate Fitness Score.
         * 
         */

        static void Main(string[] args)
        {
            
            // Graph g = new Graph("test.txt");
            Graph g = new Graph("anna.txt");
            //g.saveToFile("xD.txt");
            // g.print();

            //Console.WriteLine("Zachłanny algorytm: " + test.greedy());
            //Console.WriteLine("Test = " + test.result.Max());

            Genetics nowy = new Genetics();
            nowy.graph = g;
            nowy.letsGoGenetic(50, 10000);

            Console.WriteLine("zachłanny algorytm: " + g.greedy());

            Console.ReadKey();
        }
    }
}
