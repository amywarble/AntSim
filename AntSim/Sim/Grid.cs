using System;
using System.Collections.Generic;

namespace AntSim.Sim
{
    public class Grid
    {
        private const int WallOffset = 1;

        private readonly Cell[,] _cells;
        private readonly Dictionary<Creature, Cell> _creatures = new();

        public int Width { get; }
        public int Height { get; }

        public Grid(int width, int height)
        {
            if ((Width = width) < 1) throw new ArgumentOutOfRangeException(nameof(width));
            if ((Height = height) < 1) throw new ArgumentOutOfRangeException(nameof(height));

            _cells = new Cell[width + 2 * WallOffset, height + 2 * WallOffset];

            /*
             * Surround the world with walls to simply the logic.
             * For a 2x3 playable space, we use a 4x5 grid like:
             *
             *     W W W W
             *     W o o W        W = wall
             *     W o o W        o = space
             *     W o o W
             *     W W W W
             */
            for (var x = 0; x < width + 2 * WallOffset; x++)
            {
                for (var y = 0; y < height + 2 * WallOffset; y++)
                {
                    var c = _cells[x, y] = new Cell(this, x, y)
                    {
                        IsWall = x < WallOffset
                                 || y < WallOffset
                                 || x > width + WallOffset
                                 || y > height + WallOffset
                    };

                    c.CreatureAdded += CreatureAdded;
                    c.CreatureRemoved += CreatureRemoved;
                }
            }
        }


        private void CreatureAdded(object sender, Creature e)
        {
            if (sender is not Cell addedToCell)
                return;

            if (!_creatures.TryGetValue(e, out var gridCell))
            {
                _creatures.Add(e, addedToCell);
                return;
            }

            if (gridCell != addedToCell)
                throw new InvalidOperationException("Creature added to two different cells.");
        }


        private void CreatureRemoved(object sender, Creature e)
        {
            if (sender is not Cell removedFromCell)
                return;

            if (!_creatures.TryGetValue(e, out var gridCell))
                throw new InvalidOperationException("Untracked creature was removed from cell.");

            if (removedFromCell != gridCell)
                throw new InvalidOperationException("Creature removed from the wrong cell.");

            _creatures.Remove(e);
        }

        public Cell this[int x, int y] => _cells[x + WallOffset, y + WallOffset];

        public override string ToString() => $"Inner: ({Width}, {Height}), Total: ({_cells.GetLength(0)}, {_cells.GetLength(1)})";

        public bool TryGetCreatureCell(Creature creature, out Cell cell) => _creatures.TryGetValue(creature, out cell);
    }
}