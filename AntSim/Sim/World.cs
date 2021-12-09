using AntSim.Util;
using Redzen.Random;
using SharpNeat.BlackBox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSim.Sim
{
    public class World
    {
        private readonly List<Creature> _creatures;
        private readonly List<FoodPellet> _food = new();
        private readonly AtanLookup _lookup;
        private readonly IRandomSource _rand;

        private IEnumerable<IWorldEntity> AllEntities => ((IEnumerable<IWorldEntity>)_creatures).Concat(_food);

        public int Width { get; init; }
        public int Height { get; init; }
        public int SensorRange { get; init; }
        public double SignalThreshold { get; init; } = 0.1d;

        public World(
            int width,
            int height,
            int sensorRange,
            IRandomSource rand,
            IEnumerable<IBlackBox<double>> brains)
        {
            if ((Width = width) < 20) throw new ArgumentOutOfRangeException(nameof(width), width, "Cannot be less than 20.");
            if ((Height = height) < 20) throw new ArgumentOutOfRangeException(nameof(height), height, "Cannot be less than 20.");
            SensorRange = sensorRange;

            _rand = rand ?? throw new ArgumentNullException(nameof(rand));
            _lookup = new AtanLookup(height, width);
            _creatures = brains.Select(x => new Creature(x, this)).ToList();

            if (_creatures.Count > (width - 4) * (height - 4) - 1)
            {
                throw new ArgumentException($"{_creatures.Count} brains is too many for a ({width} {height}) grid.", nameof(brains));
            }

            RandomizePositions();
        }

        public void Step()
        {
            Draw();
            foreach (var creature in _creatures)
            {
                creature.Step();
            }
        }


        public void Draw()
        {
            Console.Clear();
            foreach (var c in _creatures)
            {
                Console.SetCursorPosition(c.X, c.Y);
                Console.Write("o");
            }
        }



        public Creature GetCreatureAt(int x, int y)
        {
            return _creatures.FirstOrDefault(c => c.X == x && c.Y == y);
        }



        private void RandomizePositions()
        {
            var i = 0;
            var movedCreatures = new List<Creature>();

            foreach (var creature in _creatures)
            {
                do
                {
                    creature.X = _rand.Next(Width - 4) + 2;
                    creature.Y = _rand.Next(Height - 4) + 2;
                } while (movedCreatures.Any(c => c.X == creature.X && c.Y == creature.Y));

                creature.Id = ++i;
                movedCreatures.Add(creature);
            }
        }

        //const float PI_over_8 = MathF.PI / 8f;
        //const float Four_over_PI = 4f / MathF.PI;
        //const float Quarter = 1f / 4f;

        //// Calc prey's position relative to the agent, in polar coordinates.
        //CartesianToPolar(
        //    _preyPos - _agentPos,
        //    out int relPosRadiusSqr,
        //    out float relPosAzimuth);



        //// Test if prey is in sensor range.
        //if (relPosRadiusSqr <= _sensorRangeSqr)
        //{
        //    // Determine which sensor segment the prey is within - [0,7]. There are eight segments and they are tilted 22.5 degrees (half a segment)
        //    // such that due North, East South and West are each in the centre of a sensor segment (rather than on a segment boundary).
        //    float thetaAdjusted = relPosAzimuth - PI_over_8;
        //    if (thetaAdjusted < 0f) thetaAdjusted += 2f * MathF.PI;
        //    int segmentIdx = 1 + (int)MathF.Floor(thetaAdjusted * Four_over_PI);

        //    // Set sensor segment's input.
        //    inputVec[segmentIdx] = 1.0;
        //}

        //// Prey closeness detector.
        //inputVec[9] = relPosRadiusSqr <= 4 ? 1.0 : 0.0;


    }
}
