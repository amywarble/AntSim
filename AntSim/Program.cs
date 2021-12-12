using AntSim.Eval;
using AntSim.Sim;
using AntSim.Util;
using Serilog;
using SharpNeat.Neat;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.DistanceMetrics.Double;
using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Genome.Double;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;
using SharpNeat.Neat.Reproduction.Sexual;
using SharpNeat.Neat.Speciation.GeneticKMeans.Parallelized;
using SharpNeat.NeuralNets;


namespace AntSim
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Debug()
                .CreateLogger();

            var decoder = NeatGenomeDecoderFactory.CreateGenomeDecoderCyclic(1, true);
            var actFnFactory = new DefaultActivationFunctionFactory<double>(true);

            var metaNeatGenome = new MetaNeatGenome<double>(
                inputNodeCount: Creature.InputNodeCount,
                outputNodeCount: Creature.OutputNodeCount,
                isAcyclic: false,
                activationFn: actFnFactory.GetActivationFunction(ActivationFunctionId.LeakyReLU.ToString()),
                connectionWeightScale: 5.0);

            var neatPop = NeatPopulationFactory<double>.CreatePopulation(
                metaNeatGenome,
                connectionsProportion: 0.05,
                popSize: 200);

            var metric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);

            var settings = new NeatEvolutionAlgorithmSettings()
            {
                ElitismProportion = 0.66,
                InterspeciesMatingProportion = 0.01,
                OffspringAsexualProportion = 0.5,
                OffspringSexualProportion = 0.5,
                SelectionProportion = 0.66,
                SpeciesCount = 15
            };

            var worldFactory = new WorldFactory();

            var ea = new NeatEvolutionAlgorithm<double>(
                settings,
                new CreatureGenomeListEvaluator(worldFactory, decoder),
                new GeneticKMeansSpeciationStrategy<double>(metric, 5, 4),
                neatPop,
                new RelativeComplexityRegulationStrategy(10, 30),
                new NeatReproductionAsexualSettings(),
                new NeatReproductionSexualSettings(),
                WeightMutationSchemeFactory.CreateDefaultScheme(5.0));

            ea.Initialise();
            ea.PerformOneGeneration();
        }
    }
}
