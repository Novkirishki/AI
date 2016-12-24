using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Decision_trees
{
    class Program
    {
        private const int MIN_NUMBER_OF_DATA_EXAMPLES = 15;
        static void Main(string[] args)
        {
            const string FileName = "../../../data.txt";
            var attributes = GetAttributes(FileName);
            var data = GetData(FileName);
            var classAttribute = attributes.Last();
            attributes.Remove(classAttribute);
            var splitData = SplitData(data);
            double success = 0;
            foreach (var testData in splitData)
            {
                var tempAttributes = new Attribute[attributes.Count];
                attributes.CopyTo(tempAttributes);
                success += Validate(testData, splitData, classAttribute, tempAttributes.ToList());
            }

            Console.WriteLine("Average: {0:F2}%", success / (double)splitData.Count);
        }

        private static double Validate(IList<string[]> testData, List<IList<string[]>> splitData, Attribute classAttribute, List<Attribute> attributes)
        {
            var learnData = splitData.Where(x => x != testData).SelectMany(x => x).ToList();
            var decisionTree = BuildTree(learnData, classAttribute, attributes);

            var guessed = 0;
            foreach (var testExample in testData)
            {
                var currentNode = decisionTree;
                while (currentNode.Attribute != null)
                {
                    currentNode = currentNode.Children.Where(x => x.Value == testExample[currentNode.Attribute.Index]).First();
                }

                if (testExample[classAttribute.Index] == currentNode.Children.First().Value)
                    guessed++;
            }

            var success = (guessed * 100) / (double)testData.Count;
            Console.WriteLine("Guessed: {0}/{1} - {2:F2}%", guessed, testData.Count, success);
            return success;
        }

        private static List<string[]> GetData(string fileName)
        {
            var data = new List<string[]>();
            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                if (line.FirstOrDefault() != '\'')
                    continue;

                var skipIndividual = false;
                var individualData = line.Split(new char[] { ',', '\'' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var value in individualData)
                    if (value == "?")
                    {
                        skipIndividual = true;
                        break;
                    }

                if (!skipIndividual)
                    data.Add(individualData);
            }

            return data;
        }

        private static List<Attribute> GetAttributes(string fileName)
        {
            var attributes = new List<Attribute>();
            var lines = File.ReadAllLines(fileName);
            var index = 0;
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    break;

                var attributeData = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var attributeValues = attributeData[2].Split(new char[] { '{', ',', '}', '\'' }, StringSplitOptions.RemoveEmptyEntries);
                attributes.Add(new Attribute
                {
                    Index = index,
                    PossibleValues = attributeValues
                });

                index++;
            }

            return attributes;
        }

        private static Node BuildTree(IList<string[]> data, Attribute classAttribute, List<Attribute> attributes)
        {
            var node = new Node();
            // if all examples are from the same class
            var firstClassValue = classAttribute.PossibleValues.First();
            var secondClassValue = classAttribute.PossibleValues.Last();
            var mostCommonClassInData = data.GroupBy(x => x[classAttribute.Index]).OrderByDescending(x => x.Count()).First().FirstOrDefault()[classAttribute.Index];
            var individualsFromFirstClassCount = data.Where(x => x[classAttribute.Index] == firstClassValue).Count();
            if (individualsFromFirstClassCount == data.Count)
                node.Value = firstClassValue;
            else if (individualsFromFirstClassCount == 0)
                node.Value = secondClassValue;
            else if (attributes.Count == 0)
                // losho mi e ot dolniq izraz
                node.Value = mostCommonClassInData;
            else
            {
                var bestAttribute = FindBestAttribute(data, classAttribute, attributes);
                node.Attribute = bestAttribute;
                foreach (var possibleValue in bestAttribute.PossibleValues)
                {
                    var childNode = new Node()
                    {
                        Value = possibleValue,
                        Children = new List<Node>()
                    };

                    if (node.Children == null)
                        node.Children = new List<Node>();

                    node.Children.Add(childNode);

                    var dataSubset = data.Where(x => x[bestAttribute.Index] == possibleValue).ToList();
                    if (dataSubset.Count <= MIN_NUMBER_OF_DATA_EXAMPLES)
                    {
                        var leaf = new Node()
                        {
                            Value = mostCommonClassInData
                        };

                        childNode.Children.Add(leaf);
                    }
                    else
                    {
                        var childTreeAttributes = attributes.ToList();
                        childTreeAttributes.Remove(bestAttribute);
                        var subTree = BuildTree(dataSubset, classAttribute, childTreeAttributes);
                        if (subTree.Children != null)
                            childNode.Children.AddRange(subTree.Children);
                        else
                            childNode.Children.Add(subTree);

                        childNode.Attribute = subTree.Attribute;
                    }
                }
            }

            return node;
        }

        private static Attribute FindBestAttribute(IList<string[]> data, Attribute classAttribute, List<Attribute> attributes)
        {
            double minEntropy = 100;
            Attribute minEntropyAttribute = null;
            foreach (var attribute in attributes)
            {
                double entropy = 0;
                foreach (var currentValue in attribute.PossibleValues)
                {
                    var examplesWithCurrentValue = data.Where(x => x[attribute.Index] == currentValue).ToList();
                    var proportion = examplesWithCurrentValue.Count() / (double)data.Count;
                    entropy += (proportion * CalcEntropy(examplesWithCurrentValue, classAttribute));
                }

                if (entropy < minEntropy)
                {
                    minEntropy = entropy;
                    minEntropyAttribute = attribute;
                }
            }

            return minEntropyAttribute;
        }

        private static double CalcEntropy(IList<string[]> data, Attribute attribute)
        {
            double entropy = 0;
            if (data.Count == 0)
                return entropy;

            foreach (var currentValue in attribute.PossibleValues)
            {
                var examplesWithCurrentValue = data.Where(x => x[attribute.Index] == currentValue).Count();
                var proportion = examplesWithCurrentValue / (double)data.Count;
                if (proportion != 0)
                    entropy -= (proportion * Math.Log(proportion, 2));
            }

            return entropy;
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

        private class Attribute
        {
            public int Index { get; set; }

            public string[] PossibleValues { get; set; }
        }

        private class Node
        {
            public Attribute Attribute { get; set; }

            public List<Node> Children { get; set; }

            public string Value { get; set; }
        }
    }
}
