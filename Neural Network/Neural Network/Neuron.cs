using System;
using System.Collections.Generic;

namespace Neural_Network
{
    internal class Neuron
    {
        public Neuron (double value = 0)
        {
            this.Value = value;
        }

        public double Activate()
        {
            double inputValue = 0;
            foreach (var connection in this.BackwardsConnections)
            {
                inputValue += (connection.FrontNeuron.Value * connection.Weight);
            }

            // do some magic here
            this.Value = 1 / (1 + Math.Pow(Math.E, -inputValue));
            return this.Value;
        }

        public double CalculateError()
        {
            double sumOfErrorsOfNextLayer = 0;
            foreach (var connection in this.ForwarConnections)
            {
                sumOfErrorsOfNextLayer += (connection.BackNeuron.Error * connection.Weight);
            }

            this.Error = this.Value * (1 - this.Value) * sumOfErrorsOfNextLayer;
            return this.Error;
        }

        public List<Connection> BackwardsConnections { get; set; }

        public List<Connection> ForwarConnections { get; set; }

        public double Value { get; set; }

        public double Error { get; set; }
    }
}
