using SharpNeat.BlackBox;
using SharpNeat.Neat.Genome;
using System;

namespace AntSim.Sim
{
    public class Creature
    {
        public const int InputNodeCount = 5;
        public const int OutputNodeCount = 4;

        public NeatGenome<double> Genome { get; }
        public IBlackBox<double> Brain { get; }

        public int Health { get; set; }
        public int Energy { get; set; }

        public Cell Location { get; set; }


        public Creature(NeatGenome<double> genome, IBlackBox<double> brain)
        {
            Genome = genome ?? throw new ArgumentNullException(nameof(genome));
            Brain = brain ?? throw new ArgumentNullException(nameof(brain));

            if (Brain.InputCount != InputNodeCount) throw new ArgumentException($"Input count must be {InputNodeCount}.  Value={Brain.InputCount}", nameof(brain));
            if (Brain.OutputCount != OutputNodeCount) throw new ArgumentException($"Output count must be {OutputNodeCount}.  Value={Brain.OutputCount}.", nameof(brain));
        }


        public void Perceive(World world)
        {
            Brain.InputVector.Reset();

            /*
             * A bias node provides considerable flexibility to a neural network model.
             * https://stats.stackexchange.com/questions/185911/why-are-bias-nodes-used-in-neural-networks
             */
            Brain.InputVector[0] = 1.0d;

            // Distance to the North wall
            Brain.InputVector[1] = Clamp((world.Height - Location.Y - 1) / world.SensorRange);

            // Distance to the East wall
            Brain.InputVector[2] = Clamp((world.Width - Location.X - 1) / world.SensorRange);

            // Distance to the South wall
            Brain.InputVector[3] = Clamp(Location.Y / world.SensorRange);

            // Distance to the West wall
            Brain.InputVector[4] = Clamp(Location.X / world.SensorRange);

            // Energy
            //Brain.InputVector[5] = Clamp(1.0 * Energy / world.MaxEnergy);
        }


        public void Think() => Brain.Activate();


        public void Act(World world)
        {
            ReadOutput(out var output, out var signal);

            if (output < 0 || signal < world.SignalThreshold)
                return;

            var targetCell = output switch
            {
                0 => Location.Grid[Location.X, Location.Y + 1],    // Move North
                1 => Location.Grid[Location.X, Location.Y - 1],    // Move South
                2 => Location.Grid[Location.X + 1, Location.Y],    // Move East
                3 => Location.Grid[Location.X - 1, Location.Y],    // Move West
                _ => null
            };

            if (targetCell is { IsWall: false, Creature: null } && targetCell != Location)
            {
                Location.Creature = null;
                targetCell.Creature = this;
                Location = targetCell;
            }
        }


        private void ReadOutput(out int idx, out double signal)
        {
            idx = 0;
            signal = Brain.OutputVector[0];

            for (var i = 1; i < Brain.OutputCount; i++)
            {
                var val = Brain.OutputVector[i];
                if (val > signal)
                {
                    signal = val;
                    idx = i;
                }
                else if (val == signal)
                {
                    idx = -1;
                }
            }
        }

        private static double Clamp(double value) =>
            value switch
            {
                < 0 => 0,
                > 1 => 1,
                _ => value
            };
    }
}
