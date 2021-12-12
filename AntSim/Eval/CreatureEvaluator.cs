using AntSim.Sim;
using SharpNeat.Evaluation;
using System;

namespace AntSim.Eval
{
    public class CreatureEvaluator : IPhenomeEvaluator<Creature>
    {
        private readonly World _world;

        public CreatureEvaluator(World world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
        }

        public FitnessInfo Evaluate(Creature creature)
        {
            var current = creature.Location;
            if (current == null)
                return FitnessInfo.DefaultFitnessInfo;

            return new FitnessInfo(1.0 * current.X / current.Grid.Width);
        }
    }
}
