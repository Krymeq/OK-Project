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
        public LinkedList<int[]> result;
        public int maxColor;                  // variable containing maximum color which is the current target to reach
        /**
         * Function which generates population of n (given parameter) chromosomes
         * with random values
         */
        
        private int validateResult(int[] res)
        {
            int fitness = 0;

            for (int i = 0; i < graph.adj.Length; i++)
            {
                foreach (int j in graph.adj[i])
                {
                    // Iterates every element in every list.
                    // If its colors are equal, a point is granted.
                    if (res[i] == res[j])
                        ++fitness;
                }
            }
            return fitness / 2; // 'cuz it calculates twice the same edge
        }


        private void generatePopulation(int amount)
        {
            // creating proper arrays
            solutions = new int[amount, graph.V];
            fitness = new int[amount];

            int nodes = graph.V;
            int colours = maxColor;

            // get the expected amount of colours needed by taking the max 
            // amount of neighbors out of every vertex in the graph
            //for (int i = 0; i < nodes; i++)
            //{
            //    if (graph.adj[i].Count() > colours)
            //    colours = graph.adj[i].Count();
            //}

            // generate n chromosomes with randomly assigned colour from the range of
            // 0..(colours / 4)

            for (int i = 0; i < amount; i++)
            {
                //Console.Write("Rozwiązanie " + i + ": ");
                for (int j = 0; j < nodes; j++)
                {
                    solutions[i, j] = rand.Next(colours);
                    //Console.Write(solutions[i,j] + ", ");
                }
                //Console.WriteLine();
            }
        }

        /**
         * Function which replaces colors out of given range with
         * random colors within range and updates fitness score.
         */
        private void cutColors()
        {
            for (int i = 0; i < solutions.GetLength(0); i++)
            {
                for (int j = 0; j < solutions.GetLength(1); j++)
                {
                    if(solutions[i,j] > maxColor)
                    {
                        solutions[i, j] = rand.Next(maxColor);
                    }
                }
                fitnessScore(i);
            }
        }


        private void saveResult()
        {
            result = new LinkedList<int[]>();
            for (int i = 0; i < solutions.GetLength(0); i++)
            {
                if (fitness[i] == 0)
                {
                    int[] arr = new int[solutions.GetLength(1)];
                    
                    for(int j = 0; j < solutions.GetLength(1); j++)
                    {
                        arr[j] = solutions[i, j];
                    }

                    result.AddLast(arr);
                }
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
                    // Iterates every element in every list.
                    // If its colors are equal, a point is granted.
                    if (solutions[index,i] == solutions[index,j])
                        ++fitness[index];
                }
            }
            fitness[index] /= 2; // 'cuz it calculates twice the same edge
        }

        /**
         * Function that select best fitting parents from 
         * two random pairs of chromosomes.
         * Used when best fitness greater than 4.
         */
        private Tuple<int, int> parentSelect1()
        {
            int index1, index2;
            
            // pick two indexes for the first pair
            int tempPar1 = rand.Next(fitness.Length);
            int tempPar2 = rand.Next(fitness.Length);
            
            // ensure that they are not equal
            while(tempPar1 == tempPar2)
            {
                tempPar2 = rand.Next(fitness.Length);
            }


            // get the better one
            if (fitness[tempPar1] < fitness[tempPar2])
                index1 = tempPar1;
            else
                index1 = tempPar2;

            // repeat for the second pair
            tempPar1 = rand.Next(fitness.Length);
            tempPar2 = rand.Next(fitness.Length);

            // ensure that they are not equal
            while (tempPar1 == tempPar2)
            {
                tempPar2 = rand.Next(fitness.Length);
            }

            if (fitness[tempPar1] < fitness[tempPar2])
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
            int index1 = 0;
            var availableIndexes = new LinkedList<int>();

            // fill availableIndexes with content
            for (int i = 0; i < solutions.GetLength(0); i++)
            {
                availableIndexes.AddLast(i);
            }
            
            // find first index
            for (int i = 0; i < availableIndexes.Count; i++)
            {
                int newIndex = availableIndexes.ElementAt(i);
                if(fitness[newIndex] < fitness[index1])
                {
                    index1 = newIndex;
                }
            }

            availableIndexes.Remove(index1);

            int index2 = availableIndexes.ElementAt(0);

            // find second index
            for (int i = 0; i < availableIndexes.Count; i++)
            {
                int newIndex = availableIndexes.ElementAt(i);
                if (fitness[newIndex] < fitness[index2])
                {
                    index2 = newIndex;
                }
            }

            return new Tuple<int, int> (index1, index2);
        }


        /**
         * Function picking two random parents (when each chromosome has equal fitness)
         * to prevent picking the same chromosomes and entering a loop
         */
        private Tuple<int, int> parentSelect3()
        {
            int index1 = rand.Next(fitness.Length);
            int index2 = rand.Next(fitness.Length);

            // ensure they are not equal
            while(index1 == index2)
            {
                index2 = rand.Next(fitness.Length);
            }
            return new Tuple<int, int>(index1, index2);
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

            // Take elements before the crosspoint from the first parent, and after it - from the second.
            for (int i = 0; i <= crosspoint; i++)
                child[i] = solutions[index1, i];

            for (int i = crosspoint + 1; i < graph.V; i++)
                child[i] = solutions[index2,i];

            return child;
        }

        /**
         * Function replacing the worst fitting chromosome with new child.
         * Returns index of child's new position.
         */
        private int offspring(int[] result)
        {
            var worstIndexes = new LinkedList<int>();
            int maxFittness = fitness[0];

            worstIndexes.AddLast(0);

            // Find worst chromosome's index
            for (int i = 1; i < fitness.Length; ++i)
            {
                if (fitness[i] > maxFittness)
                {
                    maxFittness = fitness[i];
                    worstIndexes = new LinkedList<int>();
                    worstIndexes.AddLast(i);
                }
                else if (fitness[i] == maxFittness)
                {
                    worstIndexes.AddLast(i);
                }
            }

            int leastFittestIndex = worstIndexes.ElementAt(rand.Next(worstIndexes.Count));

            // Substitute worst chromosome with the new child
            for (int i = 0; i < solutions.GetLength(1); i++)
            {
                solutions[leastFittestIndex, i] = result[i];
            }

            return leastFittestIndex;
        }

        /**
         * Function which causes mutation of item at [mutationIndex].
         * Replaces colors of adjacent colors if able.
         */
        private void mutation(int mutationIndex)
        {
            LinkedList<int> adjacentColors = new LinkedList<int>(); // empty array of all adjacent colors
            LinkedList<int> allColors = new LinkedList<int>(); // empty array of all colors


            // fill allColors with maximum number of colors
            for (int i = 0; i < maxColor; i++) 
                allColors.AddLast(i);

            // for every vertex
            for (int i = 0; i < graph.V; i++)
            {
                adjacentColors.Clear();

                // fill adjacentColors
                // iterate through adjacent vertices and add colours of the neighbors
                foreach (int item in graph.adj[i]) 
                    adjacentColors.AddLast(solutions[mutationIndex, item]);

                // if any vertex has the same colour as the given one
                if (adjacentColors.Contains(solutions[mutationIndex, i]))
                {
                    // mutate - swap colour of the vertex with the smallest colour available
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
                    if (validColors.Count > 0)
                    {
                        int newColour = rand.Next(validColors.Count);
                        solutions[mutationIndex, i] = validColors.ElementAt(newColour);
                    }
                }
            }

        }


        public void letsGoGenetic(int amountChromosomes, int maxGenerations)
        {
            int generation = 0;
            int deadEndGenerations = 0;

            maxColor = graph.greedy();
            result = new LinkedList<int[]>();
            result.AddLast(graph.result);

            Console.WriteLine();
            Console.WriteLine("Max color: " + maxColor.ToString());
            
            // void generatePopulation(int amount);
            // Generate x initial chromosomes
            generatePopulation(amountChromosomes);

            // Calculate their initial score
            // void fitnessScore(int index);
            for (int i = 0; i < amountChromosomes; i++)
                fitnessScore(i);

            //Console.WriteLine("At the beginning");
            //for (int i = 0; i < graph.V; i++)
            //{
            //    Console.Write(solutions[0, i] + ", ");
            //}
            //Console.WriteLine("\n");

            while (generation < maxGenerations)
            {
                // code somehow enters a "dead end", so after x generations 
                // during which fitness does not change, just replace it with new population
                if(fitness.Min() == fitness.Max())
                {
                    deadEndGenerations++;
                }
                else
                {
                    deadEndGenerations = 0;
                }

                if(deadEndGenerations > 100)
                {
                    deadEndGenerations = 0;
                    generatePopulation(amountChromosomes);

                    for(int i = 0; i < amountChromosomes; i++)
                    {
                        fitnessScore(i);
                    }
                }

                if(generation % 200 == 0)
                {
                    Console.WriteLine("generation " + generation);
                    //for (int i = 0; i < graph.V; i++)
                    //{
                    //    Console.Write(solutions[0, i] + ", ");
                    //}
                    Console.WriteLine();
                }
                

                generation++;
                //Console.WriteLine("Generation " + generation);

                // Tuple<int, int> parentSelect1();
                // Tuple<int, int> parentSelect2();
                Tuple<int, int> selection;
                if (fitness.Max() > 5)
                    selection = parentSelect1();
                else if (fitness.Max() != fitness.Min())
                    selection = parentSelect2();
                else
                    selection = parentSelect3();

                //Console.WriteLine("Parents : " + selection.Item1 + " and " + selection.Item2);

                // int[] crossover(int index1, int index2);
                int[] child = crossover(selection.Item1, selection.Item2);

                // Position of a new children
                // int offspring(int[] result);
                int index = offspring(child);

                // void mutation1(int mutationIndex);
                // void mutation2(int mutationIndex);
                mutation(index);

                fitnessScore(index);
                //Console.Write("Min fitness == " + fitness.Min());
                //Console.Write(". Max fitness == " + fitness.Max());
                //Console.WriteLine(". New child on index " + index + "!!!!");

                // if more than a half of solutions has fitness below 3% of vertices and there is at least one 
                // perfect one, then try to achieve a solution with lesser colours.
                // Also, save current array of solutions as it most likely will be lost during the cut.

                bool thereIsPerfect = false;
                int points = 0;

                foreach (int i in fitness)
                {
                    if(i == 0)
                    {
                        thereIsPerfect = true;
                    }

                    if(i < (0.05 * graph.V))
                    {
                        points++;
                    }
                }

                if ((points >= 0.5 * amountChromosomes) && thereIsPerfect)
                {
                    //Console.WriteLine(" ========================================== ");
                    //for (int i = 0; i < amountChromosomes; i++)
                    //{
                    //    Console.Write("Rozwiązanie[" + i + "] : ");
                    //    for (int j = 0; j < graph.V; j++)
                    //    {
                    //        Console.Write(solutions[i, j] + " ");
                    //    }

                    //    Console.Write(", Fitness: " + fitness[i]);
                    //    Console.WriteLine();
                    //}
                    //Console.WriteLine(" ========================================== ");

                    maxColor -= (int)Math.Ceiling(0.05 * maxColor);  // so that it will always be reduced by at least 1 if maxColor is positive
                    //Console.WriteLine("NOWY KOLOR TUTUTUTUTUTUTUT: " + maxColor.ToString());
                    saveResult();
                    cutColors();
                }
            }
            //Console.WriteLine("Solution found in " + generation + ". generation!");
            
            int minColors = graph.V;
            int minIndex = -1;
            int colors;
            bool perf = false;

            for (int i = 0; i < amountChromosomes; i++)
            {
                fitnessScore(i);
                if (fitness[i] == 0)
                    perf = true;
            }

            if (perf) saveResult();
                    
            
            
            //// Wypisanie rozwiązań
            //for (int i = 0; i < amountChromosomes; i++)
            //{
            //    Console.Write("Rozwiązanie[" + i + "] : ");
            //    for (int j = 0; j < graph.V; j++)
            //    {
            //        Console.Write(solutions[i, j] + " ");
            //    }
            //    Console.WriteLine();
            //}

            Console.WriteLine(" ========================================== ");

            for (int i = 0; i < result.Count; i++)
            {
                colors = -1;
                
                Console.Write("Rozwiązanie[" + i + "] : ");
                for (int j = 0; j < result.ElementAt(i).Length; j++)
                { 
                    if (result.ElementAt(i)[j] > colors)
                    colors = result.ElementAt(i)[j];
                    Console.Write(result.ElementAt(i)[j] + " ");
                }
                
                if (colors < minColors || (colors == minColors && fitness[i] < fitness[minIndex]))
                {
                    minColors = colors;
                    minIndex = i;
                }

                Console.Write(" Fitness: " + validateResult(result.ElementAt(i)));
                Console.WriteLine();
            }

            Console.WriteLine("Best is equal to " + (minColors + 1) + " colors, on index " + minIndex);
        }
    }
}
