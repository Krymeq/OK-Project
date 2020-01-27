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
        public int[] result;

        // Constructor used to generate a random graph with given saturation
        public Graph(int nodes, int saturation)
        {
            generate(nodes, saturation);
        }

        // Constructor reading graph from file
        public Graph(string filename)
        {
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            int v = int.Parse(file.ReadLine());

            V = v;
            result = new int[V];
            adj = new LinkedList<int>[V];
            for (int i = 0; i < v; i++)
                adj[i] = new LinkedList<int>();


            while ((line = file.ReadLine()) != null)
            {
                string[] val = line.Split(' ');
                int offset = 0;

                // so that we ignore first word in the line when it is not a number
                if (!Char.IsDigit(val[0][0])) offset++;

                addEdge(int.Parse(val[0 + offset]) - 1, int.Parse(val[1 + offset]) - 1);
            }
        }

        
        public void saveToFile(string filename)
        {
            var file = new System.IO.StreamWriter(filename);
            file.WriteLine(V);
            for(int i = 0; i < V; i++)
            {
                for(int j = 0; j < adj[i].Count; j++)
                {
                    Console.WriteLine(String.Format("{0}, {1}", i, j));
                    if(i < adj[i].ElementAt(j))
                    {
                        file.WriteLine(String.Format("{0} {1}", i + 1, adj[i].ElementAt(j) + 1));
                    }
                }
            }
            file.Close();
        }


        // Generate graph with given amount of nodes and saturation (in %)
        public void generate(int nodes, int saturation)
        {
            V = nodes;
            result = new int[V];
            adj = new LinkedList<int>[V];

            for (int i = 0; i < nodes; i++)
                adj[i] = new LinkedList<int>();

            var rnd = new Random();
            var disconnected = new LinkedList<int>();
            var connected = new LinkedList<int>();
            int edges = 0;

            for (int i = 0; i < nodes; i++)
                disconnected.AddLast(i);

            // Begin from random node
            connected.AddLast(disconnected.ElementAt(rnd.Next(disconnected.Count())));
            disconnected.Remove(connected.ElementAt(0));

            // Loop generating base of the graph - connecting all necessary nodes together
            while (disconnected.Count > 0)
            {
                // pick 2 nodes - one connected, one not - and connect them
                int node1 = disconnected.ElementAt(rnd.Next(disconnected.Count()));
                int node2 = connected.ElementAt(rnd.Next(connected.Count()));

                disconnected.Remove(node1);
                connected.AddLast(node1);

                addEdge(node1, node2);
                edges++;
            }

            // Second part of graph generation - picking randomly two nodes
            // and connecting them as long as the saturation requirement is not specified

            while (edges < ((float)(nodes * (nodes - 1)) / 2 * saturation / 100.0f))
            {
                int node1 = connected.ElementAt(rnd.Next(connected.Count()));
                int node2 = -1;

                //Console.WriteLine("In second loop");

                do
                {
                    node2 = connected.ElementAt(rnd.Next(connected.Count()));
                    //Console.WriteLine("In second loop - inner " + node2 + ", " + node1);
                } while (node1 == node2);

                if (!connection(node1, node2))
                {
                    addEdge(node1, node2);

                    edges++;
                }
            }
        }

        public bool connection(int i, int j)
        {
            for (int xd = 0; xd < adj[i].Count(); xd++)
            {
                if (adj[i].ElementAt(xd) == j) return true;
            }
            return false;
        }

        public void print()
        {
            for (int i = 0; i < V; i++)
            {
                Console.Write(i + ": ");
                for (int j = 0; j < adj[i].Count; j++)
                {
                    Console.Write(adj[i].ElementAt(j) + ", ");
                }
                Console.WriteLine();
            }
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
            bool[] avalible = Enumerable.Repeat<bool>(true, V).ToArray();

            result[0] = 0;

            for (int i = 1; i < V; i++)
            {
                //Console.WriteLine("i = " + i);
                // checking for color's avalibility
                foreach (int cr in adj[i])
                {
                    //Console.WriteLine("cr = " + cr);
                    //Console.WriteLine("result[cr] = " + result[cr]);
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
                Console.Write(i + " ");

            return result.Max() + 1;
        }

    }
}
