using System;
using System.Collections.Generic;
using System.Linq;

namespace Neural_Network
{
    internal class NeuralNetwork
    {
        private static Random rand = new Random();

        public NeuralNetwork(int inputNeuronsCount, int hiddenLayersCount, int hiddenLayersNeuronsCount, int outputNeuronsCount, double biasValue)
        {
            this.Layers = new List<List<Neuron>>();

            // create input layer
            var inputLayer = new List<Neuron>();
            this.Bias = new Neuron(biasValue);
            for (int i = 0; i < inputNeuronsCount; i++)
            {
                var inputNeuron = new Neuron();
                inputLayer.Add(inputNeuron);
            }

            this.Layers.Add(inputLayer);

            // create hidden layers
            for (int i = 0; i < hiddenLayersCount; i++)
            {
                this.CreateLayer(hiddenLayersNeuronsCount);
            }

            // create output layer
            this.CreateLayer(outputNeuronsCount);
        }

        public List<List<Neuron>> Layers { get; set; }

        public List<Neuron> InputLayer
        {
            get { return this.Layers[0]; }
            set { this.Layers[0] = value; }
        }

        public List<Neuron> OutputLayer
        {
            get { return this.Layers.Last(); }
            set { this.Layers[this.Layers.Count - 1] = value; }
        }

        public List<List<Neuron>> HiddenLayers
        {
            get { return this.Layers.Skip(1).Take(this.Layers.Count - 2).ToList(); }
        }

        public Neuron Bias { get; set; }

        public void ProcessInputs(List<double> inputs)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                this.InputLayer[i].Value = inputs[i];
            }

            foreach (var layer in this.Layers.Skip(1))
            {
                foreach (var neuron in layer)
                {
                    neuron.Activate();
                }
            }
        }

        public void Backpropagate(List<List<double>> learningInputs, List<List<double>> learningOutputs, int numberOfIterations, double maxError)
        {
            for (int i = 0; i < numberOfIterations + 1; i++)
            {
                double meanError = 0;
                for (int j = 0; j < learningInputs.Count; j++)
                {
                    var learningInput = learningInputs[j];
                    var learningOutput = learningOutputs[j];
                    this.Learn(learningInput, learningOutput);

                    var outputs = this.GetOutputs();
                    meanError += this.CalculateMeanSquaredError(outputs, learningOutput);
                }

                meanError /= learningInputs.Count;

                if (meanError < maxError || i == numberOfIterations)
                {
                    Console.WriteLine("Learning finished. The mean error is: {0}, on iteration number {1}", meanError, i);
                    return;
                }
            }
        }

        public List<double> GetOutputs()
        {
            var outputs = new List<double>();
            foreach (var neuron in this.OutputLayer)
            {
                outputs.Add(neuron.Value);
            }

            return outputs;
        }

        private double CalculateMeanSquaredError(List<double> outputValues, List<double> expectedValues)
        {
            double meanSquaredError = 0;
            for (int i = 0; i < outputValues.Count; i++)
            {
                meanSquaredError += Math.Pow(expectedValues[i] - outputValues[i], 2);
            }

            return meanSquaredError / outputValues.Count;
        }

        private void Learn(List<double> inputs, List<double> expectedResults)
        {
            this.ProcessInputs(inputs);
            for (int i = 0; i < expectedResults.Count; i++)
            {
                var result = this.OutputLayer[i].Value;
                var expectedResult = expectedResults[i];
                this.OutputLayer[i].Error = result * (1 - result) * (expectedResult - result);
            }

            for (int i = this.Layers.Count - 2; i > -1; i--)
            {
                foreach (var neuron in this.Layers[i])
                {
                    neuron.CalculateError();
                }
            }

            this.Bias.CalculateError();

            // adjust weights
             foreach (var layer in this.Layers.Skip(1))
            {
                foreach (var neuron in layer)
                {
                    foreach (var connection in neuron.BackwardsConnections)
                    {
                        connection.AdjustWeight();
                    }
                }
            }
        }

        private void CreateLayer(int neuronsCount)
        {
            var layer = new List<Neuron>();
            for (int j = 0; j < neuronsCount; j++)
            {
                var currentNeuron = new Neuron();
                currentNeuron.BackwardsConnections = new List<Connection>();

                foreach (var neuron in this.Layers[this.Layers.Count - 1])
                {
                    var connection = new Connection(rand.NextDouble() / 10 - 0.05, neuron, currentNeuron);
                    currentNeuron.BackwardsConnections.Add(connection);
                    if (neuron.ForwarConnections == null)
                    {
                        neuron.ForwarConnections = new List<Connection>();
                    }

                    neuron.ForwarConnections.Add(connection);
                }

                // add bias
                var biasConnection = new Connection(rand.NextDouble() / 10 - 0.05, this.Bias, currentNeuron);
                currentNeuron.BackwardsConnections.Add(biasConnection);
                if (this.Bias.ForwarConnections == null)
                {
                    this.Bias.ForwarConnections = new List<Connection>();
                }

                this.Bias.ForwarConnections.Add(biasConnection);           
                layer.Add(currentNeuron);
            }

            this.Layers.Add(layer);
        }
    }
}
