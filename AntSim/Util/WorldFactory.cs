using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntSim.Sim;
using Redzen.Random;

namespace AntSim.Util
{
    public class WorldFactory
    {
        public World Create()
        {
            var windowWidth = Console.WindowWidth * 2 / 3;
            var windowHeight = Console.WindowHeight * 2 / 3;

            return new World(
                width: windowWidth,
                height: windowHeight,
                sensorRange: 10,
                rand: new Xoshiro512StarStarRandom());
        }
    }
}
