using AntSim.Constants;
using SharpNeat.BlackBox;
using SharpNeat.Neat.Genome;
using System;

namespace AntSim.Sim
{
    public class Creature
    {
        public NeatGenome<double> Genome { get; }
        public IBlackBox<double> Brain { get; }

        public int Health { get; set; }
        public int Energy { get; set; }


        public Creature(NeatGenome<double> genome, IBlackBox<double> brain)
        {
            Genome = genome ?? throw new ArgumentNullException(nameof(genome));
            Brain = brain ?? throw new ArgumentNullException(nameof(brain));
        }


        public void Perceive(World world, Cell currentCell)
        {
            Brain.InputVector.Reset();

            InputBias = 1.0d;

            InputDistanceWallNorth = Clamp((world.Height - currentCell.Y - 1) / world.SensorRange);
            InputDistanceWallEast = Clamp((world.Width - currentCell.X - 1) / world.SensorRange);

            InputDistanceWallSouth = Clamp(currentCell.Y / world.SensorRange);
            InputDistanceWallWest = Clamp(currentCell.X / world.SensorRange);
        }


        public void Think() => Brain.Activate();


        private double Clamp(double value) =>
            value switch
            {
                < 0 => 0,
                > 1 => 1,
                _ => value
            };


        public void Act(World world, Cell currentCell)
        {
            ReadOutput(out var output, out var signal);

            if (signal < world.SignalThreshold)
                return;

            Cell targetCell;
            switch (output)
            {
                case CreatureOutput.MoveNorth:
                    targetCell = currentCell.Grid[currentCell.X, currentCell.Y + 1];
                    break;

                case CreatureOutput.MoveSouth:
                    targetCell = currentCell.Grid[currentCell.X, currentCell.Y - 1];
                    break;

                case CreatureOutput.MoveEast:
                    targetCell = currentCell.Grid[currentCell.X + 1, currentCell.Y];
                    break;

                case CreatureOutput.MoveWest:
                    targetCell = currentCell.Grid[currentCell.X - 1, currentCell.Y];
                    break;

                default:
                    targetCell = null;
                    break;
            }

            if (targetCell is { IsWall: false, Creature: null } && targetCell != currentCell)
            {
                currentCell.Creature = null;
                targetCell.Creature = this;
            }
        }


        private void ReadOutput(out CreatureOutput output, out double signal)
        {
            var sig = Brain.OutputVector[0];
            var idx = 0;

            for (var i = 1; i < Brain.OutputCount; i++)
            {
                var val = Brain.OutputVector[i];
                if (val > sig)
                {
                    sig = val;
                    idx = i;
                }
                else if (val == sig)
                {
                    idx = -1;
                }
            }

            output = (CreatureOutput)idx;
            signal = sig;
        }

        /// <summary>
        /// A bias node provides considerable flexibility to a neural network model.
        /// See <see href="https://stats.stackexchange.com/questions/185911/why-are-bias-nodes-used-in-neural-networks">Stack Exchange</see>
        /// </summary>
        private double InputBias
        {
            set => Brain.InputVector[0] = value;
        }

        private double InputDistanceWallNorth
        {
            set => Brain.InputVector[1] = value;
        }

        private double InputDistanceWallEast
        {
            set => Brain.InputVector[2] = value;
        }

        private double InputDistanceWallSouth
        {
            set => Brain.InputVector[3] = value;
        }

        private double InputDistanceWallWest
        {
            set => Brain.InputVector[4] = value;
        }
    }
}
