using System;
using System.Collections.Generic;
using System.Linq;

namespace KnapsackProblem
{
    public class Program
    {
        private const int INITIAL_POPULATION_MULTIPLIER = 4;
        private const int ITERATIONS_COUNT = 200;

        private static int MaxWeight;
        private static List<int> Weights;
        private static List<int> Values;
        private static List<Hromozome> population;
        private static Random RAND = new Random();

        public static void Main(string[] args)
        {
            ReadInput();
            GenerateInitialGeneration(Weights.Count);
            for (int i = 1; i <= ITERATIONS_COUNT; i++)
            {
                population.Sort();
                if (i == 10 || i % 50 == 0)
                {
                    Console.WriteLine(population[0].Value);
                }

                var parents = population.Take(population.Count / 4 + 1).ToList();
                GenerateNewPopulation(parents);
            }
        }

        private static void GenerateNewPopulation(List<Hromozome> parents)
        {
            GenerateMutations(parents);
        }

        private static void GenerateMutations(List<Hromozome> parents)
        {
            for (int i = 0; i < Weights.Count() * INITIAL_POPULATION_MULTIPLIER; i++)
            {
                var randomParent = parents[RAND.Next(0, parents.Count())];
                var randomIndex = RAND.Next(0, randomParent.Representation.Count());
                var childWeight = 0;
                var childValue = 0;
                var childRepresentation = (bool[])randomParent.Representation.Clone();
                childRepresentation[randomIndex] = !randomParent.Representation[randomIndex];
                if (randomParent.Representation[randomIndex] == true)
                {
                    childWeight = randomParent.Weight - Weights[randomIndex];
                    childValue = randomParent.Value - Values[randomIndex];
                }
                else
                {
                    childWeight = randomParent.Weight + Weights[randomIndex];
                    childValue = randomParent.Value + Values[randomIndex];
                }

                if (childWeight <= MaxWeight)
                {
                    population.Add(new Hromozome
                    {
                        Representation = childRepresentation,
                        Weight = childWeight,
                        Value = childValue
                    });
                }
            }
        }

        private static void ReadInput()
        {
            Console.WriteLine("Enter knapsack capacity and number of items separated by whitespace. Then enter each item value and weight on a separeta row:");
            var options = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            MaxWeight = int.Parse(options[0]);
            Values = new List<int>();
            Weights = new List<int>();
            for (int i = 0; i < int.Parse(options[1]); i++)
            {
                var line = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Values.Add(int.Parse(line[0]));
                Weights.Add(int.Parse(line[1]));
            }
        }

        private static void GenerateInitialGeneration(int numberOfItems)
        {
            population = new List<Hromozome>();
            // adding not so random hromozomes :D
            for (int j = 0; j < numberOfItems; j++)
            {
                var hromozome = new bool[numberOfItems];
                var weight = 0;
                var value = 0;
                hromozome[j] = true;
                weight += Weights[j];
                value += Values[j];

                if (weight <= MaxWeight)
                {
                    population.Add(new Hromozome
                    {
                        Representation = hromozome,
                        Weight = weight,
                        Value = value
                    });
                }
            }

            // adding random hromozomes 
            for (int i = 0; i < numberOfItems * INITIAL_POPULATION_MULTIPLIER; i++)
            {
                var hromozome = new bool[numberOfItems];
                var weight = 0;
                var value = 0;
                for (int j = 0; j < numberOfItems; j++)
                {
                    if (RAND.NextDouble() > 0.5)
                    {
                        hromozome[j] = true;
                        weight += Weights[j];
                        value += Values[j];
                    }
                    else
                    {
                        hromozome[j] = false;
                    }
                }

                if (weight <= MaxWeight)
                {
                    population.Add(new Hromozome
                    {
                        Representation = hromozome,
                        Weight = weight,
                        Value = value
                    });
                }
            }
        }

        private class Hromozome : IComparable<Hromozome>
        {
            public bool[] Representation { get; set; }

            public int Weight { get; set; }

            public int Value { get; set; }

            public int CompareTo(Hromozome other)
            {
                return other.Value.CompareTo(this.Value);
            }
        }
    }
}