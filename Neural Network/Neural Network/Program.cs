using System;
using System.Collections.Generic;
using System.Linq;

namespace Neural_Network
{
    class Program
    {
        private static List<List<Double>> LearningInputs = new List<List<double>>()
        {
            // And
            new List<double>() { 0, 0, -1 },
            new List<double>() { 0, 1, -1 },
            new List<double>() { 1, 0, -1 },
            new List<double>() { 1, 1, -1 },
            //// Or
            new List<double>() { 0, 0, 0 },
            new List<double>() { 0, 1, 0 },
            new List<double>() { 1, 0, 0 },
            new List<double>() { 1, 1, 0 },
            // Xor
            new List<double>() { 0, 0, 1 },
            new List<double>() { 0, 1, 1 },
            new List<double>() { 1, 0, 1 },
            new List<double>() { 1, 1, 1 }
        };

        private static List<List<Double>> LearningOutputs = new List<List<double>>()
        {
            // And
            new List<double>() { 0 },
            new List<double>() { 0 },
            new List<double>() { 0 },
            new List<double>() { 1 },
            // Or
            new List<double>() { 0 },
            new List<double>() { 1 },
            new List<double>() { 1 },
            new List<double>() { 1 },
            // Xor
            new List<double>() { 0 },
            new List<double>() { 1 },
            new List<double>() { 1 },
            new List<double>() { 0 }
        };

        static void Main(string[] args)
        {
            var neuralNetwork = new NeuralNetwork(3, 1, 3, 1, 1);
            neuralNetwork.Backpropagate(LearningInputs, LearningOutputs, 100000, 0.00001);
            Console.WriteLine("Enter values to test the neural network. Enter the values and operation value separated by space.");
            Console.WriteLine("Operation values: AND - -1, OR - 0, XOR - 1");
            while (true)
            {
                var input = Console.ReadLine().Split(' ').Select(Double.Parse).ToList();
                neuralNetwork.ProcessInputs(input);
                var outputs = neuralNetwork.GetOutputs();
                Console.WriteLine(String.Join(" ", outputs.ToArray()));
            }
        }
    }
}
