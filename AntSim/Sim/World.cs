using AntSim.Util;
using Redzen.Random;
using System;
using System.Collections.Generic;

namespace AntSim.Sim
{
    public class World
    {
        private readonly AtanLookup _lookup;
        private readonly IRandomSource _rand;
        private readonly List<Creature> _creatures;
        private readonly Grid _grid;

        public int Width { get; init; }
        public int Height { get; init; }

        /*
         * Parameters
         */
        public float SensorRange { get; }
        public int MaxHealth { get; }
        public int MaxEnergy { get; }

        /// <summary>
        /// Output neurons must exceed this activation level to trigger behavior
        /// </summary>
        public double SignalThreshold { get; init; } = 0.1d;

        public World(int width,
            int height,
            float sensorRange,
            double signalThreshold,
            int maxHealth,
            int maxEnergy,
            IRandomSource rand,
            IEnumerable<Creature> creatures)
        {
            if ((Width = width) < 10) throw new ArgumentOutOfRangeException(nameof(width), width, "Cannot be less than 10.");
            if ((Height = height) < 10) throw new ArgumentOutOfRangeException(nameof(height), height, "Cannot be less than 10.");

            _rand = rand ?? throw new ArgumentNullException(nameof(rand));
            _lookup = new AtanLookup(height, width);
            _grid = new Grid(width, height);
            _creatures = new List<Creature>(creatures);

            SignalThreshold = signalThreshold;
            SensorRange = sensorRange;
            MaxHealth = maxHealth;
            MaxEnergy = maxEnergy;
        }

        public void Step()
        {
            foreach (var creature in _creatures)
            {
                if (!_grid.TryGetCreatureCell(creature, out var cell))
                    continue;

                creature.Perceive(this);
                creature.Think();
                creature.Act(this);
            }
        }

        public void Add(Creature creature)
        {
            int x, y;
            do
            {
                x = _rand.Next(Width - 2) + 1;
                y = _rand.Next(Height - 2) + 1;
            } while (!_grid[x, y].CanCreatureMoveHere);

            creature.Init(this);
            _creatures.Add(_grid[x, y].Creature = creature);
        }

        public void AddRange(IEnumerable<Creature> creatures)
        {
            foreach (var creature in creatures)
            {
                Add(creature);
            }
        }
    }
}
