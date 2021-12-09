using AntSim.Constants;
using AntSim.Sim;
using Redzen.Random;
using SharpNeat.Neat;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.Double;
using SharpNeat.NeuralNets;
using System;
using System.Linq;
using System.Threading;


namespace AntSim
{
    class Program
    {
        static void Main(string[] args)
        {

            var decoder = NeatGenomeDecoderFactory.CreateGenomeDecoderCyclic(1, true);
            var actFnFactory = new DefaultActivationFunctionFactory<double>(true);

            var metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: Enum.GetValues<CreatureInput>().Length,
                outputNodeCount: Enum.GetValues<CreatureOutput>().Count(v => v >= 0),
                isAcyclic: false,
                activationFn: actFnFactory.GetActivationFunction(ActivationFunctionId.LeakyReLU.ToString()),
                connectionWeightScale: 5.0);

            var neatPop = NeatPopulationFactory<double>.CreatePopulation(
                metaNeatGenome,
                connectionsProportion: 0.05,
                popSize: 200);

            var world = new World(
                width: 80,
                height: 25,
                sensorRange: 10,
                rand: new Xoshiro512StarStarRandom(),
                brains: neatPop.GenomeList.Select(decoder.Decode));


            for (var i = 0; i < 30; i++)
            {
                world.Step();
                Thread.Sleep(700);
            }
        }
    }
}
