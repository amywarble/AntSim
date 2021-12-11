using System.Collections.Generic;
using AntSim.Sim;
using Redzen.Random;

namespace AntSim.Util
{
    public class WorldFactory
    {
        public int Width { get; set; } = 20;
        public int Height { get; set; } = 20;

        public float SensorRange { get; set; } = 10f;
        public int MaxHealth { get; set; } = 100;
        public int MaxEnergy { get; set; } = 100;

        /// <summary>
        /// Output neurons must exceed this activation level to trigger behavior
        /// </summary>
        public double SignalThreshold { get; set; } = 0.1d;

        public IRandomSource SourceOfRandomness { get; set; } = new Xoshiro512StarStarRandom();

        public World Create(IEnumerable<Creature> creatures)
        {
            return new World(
                Width,
                Height,
                SensorRange,
                SignalThreshold,
                MaxHealth,
                MaxEnergy,
                SourceOfRandomness,
                creatures);
        }
    }
}
