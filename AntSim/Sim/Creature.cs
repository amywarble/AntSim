using AntSim.Constants;
using SharpNeat.BlackBox;
using System;

namespace AntSim.Sim
{
    public class Creature : IWorldEntity
    {
        private readonly IBlackBox<double> _brain;
        private readonly World _world;

        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Creature(IBlackBox<double> brain, World world)
        {
            _brain = brain ?? throw new ArgumentNullException(nameof(brain));
            _world = world ?? throw new ArgumentNullException(nameof(world));
        }


        public void Step()
        {
            SetInputs();
            _brain.Activate();

            ReadOutput(out var output, out var signal);
            if (signal > _world.SignalThreshold)
            {
                Act(output);
            }
        }


        private void Act(CreatureOutput output)
        {
            switch (output)
            {
                case CreatureOutput.MoveNorth:
                    if (Y < _world.Height - 1 && _world.GetCreatureAt(X, Y + 1) == null)
                        Y++;

                    break;

                case CreatureOutput.MoveEast:
                    if (X < _world.Width - 1 && _world.GetCreatureAt(X + 1, Y) == null)
                        X++;

                    break;

                case CreatureOutput.MoveSouth:
                    if (Y > 0 && _world.GetCreatureAt(X, Y - 1) == null)
                        Y--;

                    break;

                case CreatureOutput.MoveWest:
                    if (X > 0 && _world.GetCreatureAt(X - 1, Y) == null)
                        X--;

                    break;

                case CreatureOutput.Contested:
                    break;
            }
        }

        private void SetInputs()
        {
            _brain.InputVector.Reset();

            /*
             * "A bias node provides considerable flexibility to a neural network model."
             * https://stats.stackexchange.com/questions/185911/why-are-bias-nodes-used-in-neural-networks
             */
            SetInput(CreatureInput.Bias, 1.0f);

            SetScaledSensorInput(CreatureInput.DistanceWallNorth, _world.Height - Y - 1);
            SetScaledSensorInput(CreatureInput.DistanceWallEast, _world.Width - X - 1);

            SetScaledSensorInput(CreatureInput.DistanceWallSouth, Y);
            SetScaledSensorInput(CreatureInput.DistanceWallWest, X);
        }


        private void SetInput(CreatureInput neuron, double value) => _brain.InputVector[(int)neuron] = value;


        private void SetScaledSensorInput(CreatureInput neuron, double value)
        {
            if (value < _world.SensorRange)
                SetInput(neuron, 1 - value / _world.SensorRange);
        }


        private void ReadOutput(out CreatureOutput output, out double signal)
        {
            var sig = _brain.OutputVector[0];
            var idx = 0;

            for (var i = 1; i < _brain.OutputCount; i++)
            {
                var val = _brain.OutputVector[i];
                if (val > sig)
                {
                    sig = val;
                    idx = i;
                }
                else if (val == sig)
                {
                    idx = (int)CreatureOutput.Contested;
                }
            }

            output = (CreatureOutput)idx;
            signal = sig;
        }
    }
}
