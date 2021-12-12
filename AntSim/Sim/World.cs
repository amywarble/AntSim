using AntSim.Util;
using Redzen.Random;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

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
            _rand = rand ?? throw new ArgumentNullException(nameof(rand));
            _lookup = new AtanLookup(height, width);
            _grid = new Grid(width, height);
            _creatures = new List<Creature>(creatures);

            SignalThreshold = signalThreshold;
            SensorRange = sensorRange;
            MaxHealth = maxHealth;
            MaxEnergy = maxEnergy;

            var availableCells = _grid
                .GetCells()
                .Where(x => x.CanCreatureMoveHere)
                .ToList();

            foreach (var creature in _creatures)
            {
                creature.Energy = MaxEnergy;
                creature.Health = MaxHealth;

                var idx = _rand.Next(availableCells.Count);
                var cell = availableCells[idx];
                availableCells.RemoveAt(idx);

                AssignCreatureToCell(creature, cell);
            }

            Console.WriteLine("Starting world:");
            Console.WriteLine(_grid.Print());
        }


        public void Step()
        {
            foreach (var creature in _creatures)
            {
                creature.Perceive(this);
                creature.Think();
                creature.Act(this);
            }
        }


        private void AssignCreatureToCell(Creature creature, Cell cell)
        {
            creature.Location = cell;
            cell.Creature = creature;
            Log.Information($"Creature assigned at: {cell}");
        }
    }
}
