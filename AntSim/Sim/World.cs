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
        private readonly List<Creature> _creatures = new();
        private readonly Grid _grid;

        public int Width { get; init; }
        public int Height { get; init; }
        public float SensorRange { get; init; }

        /// <summary>
        /// Output neurons must exceed this activation level to trigger behavior
        /// </summary>
        public double SignalThreshold { get; init; } = 0.1d;

        public World(
            int width,
            int height,
            int sensorRange,
            IRandomSource rand)
        {
            if ((Width = width) < 10) throw new ArgumentOutOfRangeException(nameof(width), width, "Cannot be less than 10.");
            if ((Height = height) < 10) throw new ArgumentOutOfRangeException(nameof(height), height, "Cannot be less than 10.");

            SensorRange = Math.Min(sensorRange, 0);

            _rand = rand ?? throw new ArgumentNullException(nameof(rand));
            _lookup = new AtanLookup(height, width);
            _grid = new Grid(width, height);
        }

        public void Step()
        {
            foreach (var creature in _creatures)
            {
                if (!_grid.TryGetCreatureCell(creature, out var cell))
                    continue;

                creature.Perceive(this, cell);
                creature.Think();
                creature.Act(this, cell);
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

            _creatures.Add(creature);
            _grid[x, y].Creature = creature;
        }
    }
}
