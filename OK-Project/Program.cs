using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OK_Project
{
    class Graph
    {
        public int V;
        public LinkedList<int>[] adj;

        public Graph(int v)
        {
            V = v;
            adj = new LinkedList<int>[V];
            for (int i = 0; i < v; i++)
                adj[i] = new LinkedList<int>();
        }

        public void addEdge(int v, int w)
        {
            adj[v].AddLast(w);
            adj[w].AddLast(v);
        }

        /**
         *  Algorytm zachłanny problemu kolorowania grafu.
         *  Wierzchołki są kolorowane kolejno wg numeru wierzchołka.
         *  @return colors - liczbę kolorów wykorzystanych do pokolorowania grafu w klasie.
         *  Kolory i wierzchołki indeksowane od 0.
         *  @return (max + 1).
         */
        public int greedy()
        {
            // Enumerable.Repeat<T>(x1, x2).ToArray() wypełnia tablicę wartościami x1, w ilości x2
            int[] result = Enumerable.Repeat<int>(-1, V).ToArray();
            bool[] avalible = Enumerable.Repeat<bool>(true, V).ToArray();

            result[0] = 0;
            
            for (int i = 1; i < V; i++)
            {
                Console.WriteLine("i = " + i);
                // checking for color's avalibility
                foreach (int cr in adj[i])
                {
                    Console.WriteLine("cr = " + cr);
                    Console.WriteLine("result[cr] = " + result[cr]);
                    if (result[cr] != -1)
                        avalible[result[cr]] = false;
                }
                
                for (int cr = 0; cr < V; cr++)
                    if (avalible[cr] == true)
                    {
                        result[i] = cr;
                        break;
                    }
                avalible = Enumerable.Repeat<bool>(true, V).ToArray();
            }

            foreach (int i in result)
                Console.WriteLine("Color : " + i);

            return result.Max()+1;
        }

    }
    class Program
    {
        /**
         * Najprostsza funkcja odczytywania z pliku.
         * Przykładowy format wymaganego pliku aktualnie:
         * 5 -> liczba wierzchołków
         * 1 2 -> połączenie krawędzi
         * 1 3
         * 2 4
         * 2 5
         * 3 5
         * 4 5
         * TO DO: parser do plików (e 1 2) -> edge
         **/
        static Graph LoadFromFile()
        {
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Selethen\Source\Repos\Krymeq\OK-Project\OK-Project\test.txt");
            Graph g = new Graph(int.Parse(file.ReadLine()));

            while ((line = file.ReadLine()) != null)
            {
                string[] val = line.Split(' ');
                g.addEdge(int.Parse(val[0])-1, int.Parse(val[1])-1 );
            }

            return g;
        }
        static void Main(string[] args)
        {
            Graph g = LoadFromFile();
            for (int i = 0; i < g.adj.Length; i++)
            {
                Console.Write("Vertex " + i + ": ");
                foreach (int j in g.adj[i])
                {
                    Console.Write(j + " ");
                }
                Console.WriteLine();
                Console.WriteLine("===========");
            }

            Console.WriteLine("Zachłanny algorytm: " + g.greedy());

            Console.ReadKey();
        }
    }
}
