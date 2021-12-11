using System.Collections.Generic;
using AntSim.Sim;
using SharpNeat.Evaluation;

namespace AntSim.Eval
{
    public class CreatureEvaluationScheme : IPhenomeEvaluationScheme<Creature>
    {
        public IPhenomeEvaluator<Creature> CreateEvaluator() => new CreatureEvaluator();

        public bool TestForStopCondition(FitnessInfo fitnessInfo) => false;

        public bool IsDeterministic => true;

        public IComparer<FitnessInfo> FitnessComparer => new PrimaryFitnessInfoComparer();

        public FitnessInfo NullFitness => FitnessInfo.DefaultFitnessInfo;

        public bool EvaluatorsHaveState => false;
    }
}
