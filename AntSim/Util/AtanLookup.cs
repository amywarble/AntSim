using System;

namespace AntSim.Util
{
    public class AtanLookup
    {
        private readonly float[,] _lookup;

        public int Height { get; }
        public int Width { get; }

        public AtanLookup(int height, int width)
        {
            Height = height;
            Width = width;
            _lookup = new float[height * 2 - 1, width * 2 - 1];

            for (var y = 0; y < _lookup.GetLength(0); y++)
            {
                for (var x = 0; x < _lookup.GetLength(1); x++)
                {
                    _lookup[y, x] = MathF.Atan2(y - height + 1, x - width + 1);
                }
            }
        }

        public (float radius, float azimuth) GetVector((int x, int y) posFrom, (int x, int y) posTo)
        {
            var x = posFrom.x - posTo.x;
            var y = posFrom.y - posTo.y;

            var azimuth = _lookup[y + Height - 1, x + Width - 1];
            if (azimuth < 0f)
            {
                azimuth += 2f * MathF.PI;
            }

            var radius = MathF.Sqrt(x * x + y * y);
            return (radius, azimuth);
        }
    }
}
