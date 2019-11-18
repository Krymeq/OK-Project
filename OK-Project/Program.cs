using System;
using System.Collections.Generic;
using System.IO;
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
            Graph g = new Graph(30, 70);

            //g2.saveToFile("graph1.txt");
            //g3.saveToFile("graph2.txt");
            //g4.saveToFile("graph3.txt");

            //Console.WriteLine("Zachłanny algorytm: " + test.greedy());
            //Console.WriteLine("Test = " + test.result.Max());

            Genetics nowy = new Genetics();
            nowy.graph = g;
            nowy.letsGoGenetic(50, 100);

            Console.WriteLine("Zachłanny algorytm: " + g.greedy());

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
