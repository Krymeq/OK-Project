using System;
using System.Collections.Generic;
using System.Linq;

namespace OK_Project
{
    class Graph
    {
        public int V;
        public LinkedList<int>[] adj;
        public int[] result;
        public int fitness = 0;

        // Constructor used to generate a random graph with given saturation
        public Graph(int nodes, int saturation)
        {
            generate(nodes, saturation);
        }

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
                addEdge(int.Parse(val[0]) - 1, int.Parse(val[1]) - 1);
            }
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

            for(int i = 0; i < nodes; i++)
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

            while(edges < ((float) (nodes *  (nodes - 1)) / 2 * saturation / 100.0f))
            {
                int node1 = connected.ElementAt(rnd.Next(connected.Count()));
                int node2 = -1;

                Console.WriteLine("In second loop");

                do
                {
                    node2 = connected.ElementAt(rnd.Next(connected.Count()));
                    Console.WriteLine("In second loop - inner " + node2 + ", " + node1);
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
            for(int xd = 0; xd < adj[i].Count(); xd++)
            {
                if (adj[i].ElementAt(xd) == j) return true;
            }
            return false;
        }

        public void print()
        {
            for(int i = 0; i < V; i++)
            {
                Console.Write(i + ": ");
                for(int j = 0; j < adj[i].Count; j++)
                {
                    Console.Write(adj[i].ElementAt(j) + ", ");
                }
                Console.WriteLine();
            }
        }

        public Graph clone()
        {
            Graph result = this;
            return result;
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


    class Genetics
    {
        Random rand = new Random();
        public Graph[] chromosomes = new Graph[1];

        List<LinkedList<int>> solutions;

        public void generatePopulation(int amount)
        {
            solutions = new List<LinkedList<int>>();

            int nodes = chromosomes[0].V;
            int colours = 0;

            for (int i = 0; i < nodes; i++)
            {
                if (chromosomes[0].adj[i].Count() > colours)
                    colours = chromosomes[0].adj[i].Count();
            }

            for(int i = 0; i < amount; i++)
            {
                solutions.Add(new LinkedList<int>());

                Console.Write("Rozwiązanie " + i + ": ");
                for(int j = 0; j < nodes; j++)
                {
                    solutions.ElementAt(i).AddLast(rand.Next(colours));
                    Console.Write(solutions.ElementAt(i).ElementAt(j) + ", ");
                }
                Console.WriteLine();
            }


        }
        /**
         * Function that calculates Fitness Score for given Graph
         * Fitness == 0, means that we have perfect fitness
         * In this case, as fitness score we used number of edges, that vertices have the same colour.
         */
        
        
        
        public void fitnessScore(Graph graph)
        {
            for (int i = 0; i < graph.adj.Length; i++)
            {
                foreach (int j in graph.adj[i])
                {
                    // Iterates every element in every list
                    if (graph.result[i] == graph.result[j])
                        ++graph.fitness;
                }
            }
            graph.fitness /= 2; // 'cuz calculates twice the same edge
        }
        
        /**
         * Function that select fittest parents from two random pairs of chromosomes.
         * Used when best fitness greater than 4.
         */
        public Tuple<Graph, Graph> parentSelect1()
        {
            Graph g1, g2;

            Graph tempPar1 = chromosomes[rand.Next(chromosomes.Length)];
            Graph tempPar2 = chromosomes[rand.Next(chromosomes.Length)];

            // ??????
            // not sure, if should be greater or less
            // ??????
            if (tempPar1.fitness > tempPar2.fitness)
                g1 = tempPar1; 
            else
                g1 = tempPar2;

            tempPar1 = chromosomes[rand.Next(chromosomes.Length)];
            tempPar2 = chromosomes[rand.Next(chromosomes.Length)];

            if (tempPar1.fitness > tempPar2.fitness)
                g2 = tempPar1;
            else
                g2 = tempPar2;

            return new Tuple<Graph, Graph>(g1, g2);
        }

        /**
        * Function that select two best performing chromosomes
        * Used when best fitness less than 4.
        */
        public Tuple<Graph, Graph> parentSelect2()
        {
            Graph g1, g2;
            g1 = chromosomes[0];
            g2 = chromosomes[0];

            for (int i = 1; i < chromosomes.Length; i++)
                if (g1.fitness > chromosomes[i].fitness)
                    g1 = chromosomes[i];

            for (int i = 1; i < chromosomes.Length; i++)
                if ((g2.fitness > chromosomes[i].fitness) && (g1 != chromosomes[i]))
                    g2 = chromosomes[i];
            
            return new Tuple<Graph, Graph>(g1, g2);
        }

        /**
         * Funcion that cross two parents into one child.
         * I like to call it "reproduction".
         * Crosspoint is generated at random.
         */
        public Graph crossover(Graph g1, Graph g2)
        {
            Graph child = g1;
            int crosspoint = rand.Next(child.V);

            for (int i = 0; i <= crosspoint; i++)
                child.result[i] = g1.result[i];

            for (int i = crosspoint + 1; i <= child.V; i++)
                child.result[i] = g2.result[i];

            return child;
        }

    }
    
    class Program
    {
        static void Main(string[] args)
        {
            // Graph g = new Graph("test.txt");
            Graph g = new Graph(9, 10);
            Graph test = g.clone();

            // g.print();

            //Console.WriteLine("Zachłanny algorytm: " + test.greedy());
            //Console.WriteLine("Test = " + test.result.Max());

            Genetics nowy = new Genetics();
            nowy.chromosomes[0] = g;
            nowy.generatePopulation(5);
            //nowy.fitnessScore(test);
            //Tuple<Graph, Graph> xxx = nowy.parentSelect1();

            //xxx = nowy.parentSelect1();
            //xxx = nowy.parentSelect1();

            Console.ReadKey();
        }
    }
}
