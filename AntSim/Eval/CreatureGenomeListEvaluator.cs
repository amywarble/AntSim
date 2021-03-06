using AntSim.Sim;
using AntSim.Util;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;
using SharpNeat.Neat.Genome;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSim.Eval
{
    public class CreatureGenomeListEvaluator : IGenomeListEvaluator<NeatGenome<double>>
    {
        private readonly WorldFactory _worldFactory;
        private readonly IGenomeDecoder<NeatGenome<double>, IBlackBox<double>> _decoder;


        public CreatureGenomeListEvaluator(
            WorldFactory worldFactory,
            IGenomeDecoder<NeatGenome<double>, IBlackBox<double>> decoder)
        {
            _worldFactory = worldFactory;
            _decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
        }

        public bool IsDeterministic => false;

        public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

        public void Evaluate(IList<NeatGenome<double>> genomeList)
        {
            foreach (var g in genomeList)
            {
                g.FitnessInfo = FitnessInfo.DefaultFitnessInfo;
            }

            var creatures = genomeList
                .Select(genome => new
                {
                    Genome = genome,
                    Brain = _decoder.Decode(genome)
                })
                .Where(x => x.Brain != null)
                .Select(x => new Creature(x.Genome, x.Brain))
                .ToArray();

            var world = _worldFactory.Create(creatures);

            for (var i = 0; i < 30; i++)
            {
                world.Step();
            }

            var evaluator = new CreatureEvaluator(world);
            foreach (var c in creatures)
            {
                c.Genome.FitnessInfo = evaluator.Evaluate(c);
            }
        }

        public bool TestForStopCondition(FitnessInfo fitnessInfo)
        {
            return false;
        }

        //private static void Draw(World world)
        //{
        //    Console.Clear();
        //    foreach (var c in world.Creatures)
        //    {
        //        Console.SetCursorPosition(c.X, c.Y);
        //        Console.Write("o");
        //    }
        //}
    }
}