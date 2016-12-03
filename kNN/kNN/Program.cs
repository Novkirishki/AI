using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace kNN
{
    class Program
    {
        static void Main(string[] args)
        {
            const string DataFileName = "../../../data.txt";
            Console.WriteLine("Enter the number of test flowers");
            var testDataCount = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter the number of nearest neighbours");
            var nearestNeighboursCount = int.Parse(Console.ReadLine());

            var data = ReadData(DataFileName);
            var testData = GetTestData(data, testDataCount);
            GuessType(data, testData, nearestNeighboursCount);
        }

        private static void GuessType(IList<Flower> data, HashSet<Flower> testData, int nearestNeighboursCount)
        {
            var guessedIndividuals = 0;
            foreach (var testIndividual in testData)
            {
                var nearestNeighbours = data.ToDictionary(x => x, x => x.CalculateDistanceTo(testIndividual)).OrderBy(x => x.Value).Take(nearestNeighboursCount);
                var suggestedType = nearestNeighbours.GroupBy(x => x.Key.Kind).First().Key;
                if (suggestedType == testIndividual.Kind)
                    ++guessedIndividuals;
            }

            Console.WriteLine("The algorithm guessed right {0}/{1} - {2:F2}% of the test flowers", guessedIndividuals, testData.Count, (guessedIndividuals * 100) / testData.Count);
        }

        private static HashSet<Flower> GetTestData(IList<Flower> data, int testDataCount)
        {         
            var testData = new HashSet<Flower>();
            var rand = new Random();
            while (testData.Count < testDataCount)
            {
                var individual = data[rand.Next(data.Count)];
                data.Remove(individual);
                testData.Add(individual);
            }

            return testData;
        }

        private static IList<Flower> ReadData(string fileName)
        {
            var data = new List<Flower>();
            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                var individualData = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var properties = new double[individualData.Length - 1];
                for (int i = 0; i < properties.Length; i++)
                {
                    properties[i] = double.Parse(individualData[i]);
                }

                var kind = individualData.Last();

                data.Add(new Flower(properties, kind));
            }

            return data;
        }

        private class Flower
        {
            public Flower(double[] properties, string kind)
            {
                this.Properties = properties;
                this.Kind = kind;
            }

            public double[] Properties { get; }

            public string Kind { get; }

            public double CalculateDistanceTo(Flower target)
            {
                double distance = 0;
                for (int i = 0; i < target.Properties.Length; i++)
                {
                    distance += Math.Pow(this.Properties[i] - target.Properties[i], 2);
                }

                return distance;
            }
        }
    }
}
