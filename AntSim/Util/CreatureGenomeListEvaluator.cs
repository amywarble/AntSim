using AntSim.Sim;
using SharpNeat.BlackBox;
using SharpNeat.Evaluation;
using SharpNeat.Neat.Genome;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntSim.Util
{
    public class CreatureGenomeListEvaluator : IGenomeListEvaluator<NeatGenome<double>>
    {
        private readonly WorldFactory _worldFactory;
        private readonly IGenomeDecoder<NeatGenome<double>, IBlackBox<double>> _decoder;
        private readonly IPhenomeEvaluationScheme<Creature> _evalScheme;


        public CreatureGenomeListEvaluator(
            WorldFactory worldFactory,
            IGenomeDecoder<NeatGenome<double>, IBlackBox<double>> decoder,
            IPhenomeEvaluationScheme<Creature> evalScheme)
        {
            _worldFactory = worldFactory;
            _decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            _evalScheme = evalScheme ?? throw new ArgumentNullException(nameof(evalScheme));
        }

        public bool IsDeterministic => false;

        public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

        public void Evaluate(IList<NeatGenome<double>> genomeList)
        {
            foreach (var g in genomeList)
            {
                g.FitnessInfo = _evalScheme.NullFitness;
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

            var world = _worldFactory.Create();

            foreach (var creature in creatures)
            {
                world.Add(creature);
            }

            for (var i = 0; i < 30; i++)
            {
                world.Step();
            }

            var evaluator = _evalScheme.CreateEvaluator();
            foreach (var c in creatures)
            {
                c.Genome.FitnessInfo = evaluator.Evaluate(c);
            }
        }

        public bool TestForStopCondition(FitnessInfo fitnessInfo)
        {
            return _evalScheme.TestForStopCondition(fitnessInfo);
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