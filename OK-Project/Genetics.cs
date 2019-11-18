using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OK_Project
{
    class Genetics
    {
        Random rand = new Random();
        public Graph graph;

        public int[] fitness;
        public int[,] solutions;

        private void generatePopulation(int amount)
        {
            solutions = new int[amount, graph.V];
            fitness = new int[amount];

            int nodes = graph.V;
            int colours = 0;

            for (int i = 0; i < nodes; i++)
            {
                if (graph.adj[i].Count() > colours)
                    colours = graph.adj[i].Count();
            }

            for (int i = 0; i < amount; i++)
            {
                Console.Write("Rozwiązanie " + i + ": ");
                for (int j = 0; j < nodes; j++)
                {
                    solutions[i, j] = rand.Next(colours / 4);
                    Console.Write(solutions[i,j] + ", ");
                }
                Console.WriteLine();
            }
        }

        /**
         * Function that calculates Fitness Score for given solution[index].
         * Fitness == 0, means that we have perfect fitness
         * In this case, as fitness score we used number of edges, that vertices have the same colour.
         */
        private void fitnessScore(int index)
        {
            fitness[index] = 0;
            for (int i = 0; i < graph.adj.Length; i++)
            {
                foreach (int j in graph.adj[i])
                {
                    // Iterates every element in every list
                    if (solutions[index,i] == solutions[index,j])
                        ++fitness[index];
                }
            }
            fitness[index] /= 2; // 'cuz calculates twice the same edge
        }

        /**
         * Function that select fittest parents from two random pairs of chromosomes.
         * Used when best fitness greater than 4.
         */
        private Tuple<int, int> parentSelect1()
        {
            int index1, index2;

            int tempPar1 = rand.Next(fitness.Length);
            int tempPar2 = rand.Next(fitness.Length);

            if (fitness[tempPar1] > fitness[tempPar2])
                index1 = tempPar1;
            else
                index1 = tempPar2;

            tempPar1 = rand.Next(fitness.Length);
            tempPar2 = rand.Next(fitness.Length);

            if (fitness[tempPar1] > fitness[tempPar2])
                index2 = tempPar1;
            else
                index2 = tempPar2;

            return new Tuple<int, int>(index1, index2);
        }

        /**
        * Function that select two best performing chromosomes
        * Used when best fitness less than 4.
        */
        private Tuple<int, int> parentSelect2()
        {
            int index1, index2;
            index1 = 0;
            index2 = 0;

            for (int i = 1; i < fitness.Length; i++)
                if (fitness[index1] > fitness[i])
                    index1 = i;

            for (int i = 1; i < fitness.Length; i++)
                if ((fitness[index2] > fitness[i]) && (index1 != i))
                    index2 = i;
            
            return new Tuple<int, int> (index1, index2);
        }

        /**
         * Funcion that cross two parents into one child.
         * I like to call it "reproduction".
         * Crosspoint is generated at random.
         */
        private int[] crossover(int index1, int index2)
        {
            int[] child = new int[graph.V];

            int crosspoint = rand.Next(graph.V);

            for (int i = 0; i <= crosspoint; i++)
                child[i] = solutions[index1, i];

            for (int i = crosspoint + 1; i < graph.V; i++)
                child[i] = solutions[index2,i];

            return child;
        }

        /**
         * Function replacing the least fittest chromosome with new child.
         * Returns index of child's new position.
         */
        private int offspring(int[] result)
        {
            int maxFittness = fitness[0];
            int leastFittestIndex = 0;

            for (int i = 1; i < fitness.Length; ++i)
            {
                if (fitness[i] > maxFittness)
                {
                    maxFittness = fitness[i];
                    leastFittestIndex = i;
                }
            }
            
            for (int i = 0; i < solutions.GetLength(1); i++)
            {
                solutions[leastFittestIndex, i] = result[i];
            }

            return leastFittestIndex;
        }

        /**
         * Function which causes mutation of item at [mutationIndex].
         * Replaces colors of adjacent colors if can.
         */
        private void mutation1(int mutationIndex)
        {
            LinkedList<int> adjacentColors = new LinkedList<int>(); // empty array of all adjacent colors
            LinkedList<int> allColors = new LinkedList<int>(); // empty array of all colors

            for (int i = 0; i < graph.V; i++) // fill allColors with maximum number of colors
                allColors.AddLast(i);

            for (int i = 0; i < graph.V; i++)
            {
                adjacentColors.Clear();
                foreach (int item in graph.adj[i]) // fill adjacentColors
                    adjacentColors.AddLast(solutions[mutationIndex, item]);

                if (adjacentColors.Contains(solutions[mutationIndex, i]))
                {
                    // mutate
                    LinkedList<int> validColors = new LinkedList<int>();
                    foreach (int item in allColors)
                    {
                        if (!adjacentColors.Contains(item))
                            validColors.AddLast(item);
                    }

                    /* To test: (TO DO)
                     * A) smallest colors from validColors()
                     * B) random color from validColors()
                     */
                    solutions[mutationIndex, i] = validColors.First();
                }
            }

        }

        private void mutation2(int mutationIndex)
        {
            LinkedList<int> adjacentColors = new LinkedList<int>(); // empty array of all adjacent colors
            LinkedList<int> allColors = new LinkedList<int>(); // empty array of all colors

            for (int i = 0; i < graph.V; i++) // fill allColors with maximum number of colors
                allColors.AddLast(i);

            for (int i = 0; i < graph.V; i++)
            {
                adjacentColors.Clear();
                foreach (int item in graph.adj[i]) // fill adjacentColors
                    adjacentColors.AddLast(solutions[mutationIndex, item]);

                if (adjacentColors.Contains(solutions[mutationIndex, i]))
                {
                    // mutate
                    int newColor = rand.Next(allColors.Count);
                    solutions[mutationIndex, i] = newColor;
                }
            }

        }

        public void letsGoGenetic(int amountChromosomes, int maxGenerations)
        {
            int generation = 0;
            // void generatePopulation(int amount);
            generatePopulation(amountChromosomes);

            // void fitnessScore(int index);
            for (int i = 0; i < amountChromosomes; i++)
                fitnessScore(i);

            while ((generation != maxGenerations) && (fitness.Max() != 0))
            {
                generation++;
                Console.WriteLine("Generation " + generation);

                // Tuple<int, int> parentSelect1();
                // Tuple<int, int> parentSelect2();
                Tuple<int, int> selection;
                if (fitness.Max() > 5)
                    selection = parentSelect1();
                else
                    selection = parentSelect2();

                Console.WriteLine("Parents : " + selection.Item1 + " and " + selection.Item2);

                // int[] crossover(int index1, int index2);
                int[] child = crossover(selection.Item1, selection.Item2);

                // int offspring(int[] result);
                int index = offspring(child);

                // void mutation1(int mutationIndex);
                // void mutation2(int mutationIndex);
                mutation1(index);

                fitnessScore(index);
                Console.Write("Min fitness == " + fitness.Min());
                Console.Write(". Max fitness == " + fitness.Max());
                Console.WriteLine(". New child on index " + index + "!!!!");
            }
            Console.WriteLine("Solution found in " + generation + ". generation!");
            
            int minColors = graph.V;
            int minIndex = -1;
            int colors;

            for (int i = 0; i < solutions.GetLength(0); i++)
            {
                colors = -1;
                if (fitness[i] == 0)
                {
                    Console.Write("Rozwiązanie[" + i + "] : ");
                    for (int j = 0; j < solutions.GetLength(1); j++)
                    {
                        if (solutions[i,j] > colors)
                            colors = solutions[i, j];
                        Console.Write(solutions[i, j] + " ");
                    }
                    Console.WriteLine();
                }
                if (colors < minColors)
                {
                    minColors = colors;
                    minIndex = i;
                }
            }

            Console.WriteLine("Best is equal to " + (minColors+1) + " colors, on index " + minIndex);
            
        }
    }

}
