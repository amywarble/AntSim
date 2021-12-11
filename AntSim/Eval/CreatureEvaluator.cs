using AntSim.Sim;
using SharpNeat.Evaluation;

namespace AntSim.Eval
{
    public class CreatureEvaluator : IPhenomeEvaluator<Creature>
    {
        public FitnessInfo Evaluate(Creature creature)
        {
            //var fitness = phenome.X > phenome.World.Width / 2
            //    ? 1.0
            //    : 0.0;
            return new FitnessInfo(1);
        }
    }
}
