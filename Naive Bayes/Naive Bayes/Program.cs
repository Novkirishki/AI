using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naive_Bayes
{
    class Program
    {
        static void Main(string[] args)
        {
            const string FileName = "../../../data.txt";
            var data = ReadData(FileName);
            var splitData = SplitData(data);
            double success = 0;
            foreach (var testData in splitData)
            {
                success += Validate(testData, splitData);
            }

            Console.WriteLine("Average: {0:F2}%", success / (double)splitData.Count);
        }

        private static double Validate(IList<string[]> testData, List<IList<string[]>> splitData)
        {
            var learnData = splitData.Where(x => x != testData).SelectMany(x => x);
            var attrProbabilities = CalcAttrProbabilities(learnData);
            var possibleClasses = learnData.Select(x => x.Last()).Distinct();
            var classProbabilities = new Dictionary<string, double>();
            foreach (var @class in possibleClasses)
            {
                classProbabilities.Add(@class, learnData.Where(x => x.Last() == @class).Count() / (double)learnData.Count());
            }

            var guessed = 0;
            foreach (var testIndividual in testData)
            {
                var results = new Dictionary<string, double>();
                foreach (var @class in possibleClasses)
                {
                    double probability = 1;
                    for (int i = 0; i < testIndividual.Length - 1; i++)
                    {
                        var currentProb = attrProbabilities.Where(x => x.AttrNumber == i && x.Answer == testIndividual[i] && x.Class == @class);
                        if (currentProb.Count() != 0)
                            probability *= currentProb.First().Value;
                        else
                            probability *= (1 / (double)learnData.Where(x => x.Last() == @class).Count());
                    }

                    results.Add(@class, probability * classProbabilities[@class]);
                }

                if (results.OrderByDescending(x => x.Value).First().Key == testIndividual.Last())
                    ++guessed;
            }

            var success = (guessed * 100) / (double)testData.Count;
            Console.WriteLine("Guessed: {0}/{1} - {2:F2}%", guessed, testData.Count, success);
            return success;
        }

        private static List<Probability> CalcAttrProbabilities(IEnumerable<string[]> learnData)
        {
            var attrProbs = new List<Probability>();
            var classGroups = learnData.GroupBy(x => x.Last());
            foreach (var classGroup in classGroups)
            {
                for (int attrNumber = 0; attrNumber < learnData.First().Count() - 1; attrNumber++)
                {
                    var answerGroups = classGroup.GroupBy(x => x[attrNumber]);
                    foreach (var answerGroup in answerGroups)
                    {
                        attrProbs.Add(new Probability
                        {
                            AttrNumber = attrNumber,
                            Answer = answerGroup.Key,
                            Class = classGroup.Key,
                            Value = answerGroup.Count() / (double)classGroup.Count()
                        });
                    }
                }
            }

            return attrProbs;
        }

        private static List<IList<string[]>> SplitData(IList<string[]> data)
        {
            var chunkSize = data.Count / 10;
            var rand = new Random();
            var result = new List<IList<string[]>>();
            for (int i = 0; i < 9; i++)
            {
                var currentDataChunk = new List<string[]>();
                while (currentDataChunk.Count < chunkSize)
                {
                    var individual = data[rand.Next(data.Count)];
                    data.Remove(individual);
                    currentDataChunk.Add(individual);
                }

                result.Add(currentDataChunk);
            }

            result.Add(data);
            return result;
        }

        private static IList<string[]> ReadData(string fileName)
        {
            var data = new List<string[]>();
            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                var individualData = line.Split(new char[] { ',', '\'' }, StringSplitOptions.RemoveEmptyEntries);
                data.Add(individualData);
            }

            return data;
        }

        private class Probability
        {
            public int AttrNumber { get; set; }

            public string Answer { get; set; }

            public string Class { get; set; }

            public double Value { get; set; }
        }
    }
}
