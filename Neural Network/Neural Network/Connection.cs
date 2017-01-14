namespace Neural_Network
{
    internal class Connection
    {
        public Connection(double weight, Neuron frontNeuron, Neuron backNeuron)
        {
            this.Weight = weight;
            this.FrontNeuron = frontNeuron;
            this.BackNeuron = backNeuron;
        }

        public void AdjustWeight()
        {
            this.Weight = this.Weight + 0.5 * this.BackNeuron.Error * this.FrontNeuron.Value;
        }

        public double Weight { get; set; }

        public Neuron FrontNeuron { get; set; }

        public Neuron BackNeuron { get; set; }
    }
}
