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
            var current = creature.CurrentCell;
            if (current == null)
                return FitnessInfo.DefaultFitnessInfo;

            var fitness = current.X > current.Grid.Width * 2 / 3
                ? 1.0
                : 0.0;
            return new FitnessInfo(fitness);
        }
    }
}
