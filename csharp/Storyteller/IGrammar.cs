using Storyteller.Conversion;
using Storyteller.Internal.Engine;
using Storyteller.Model;

namespace Storyteller
{
    public interface IGrammar
    {
        void CreatePlan(ExecutionPlan plan, StepValues values, FixtureLibrary library, bool inTable = false);

        GrammarModel Compile(Fixture fixture, CellHandling cells);

        string Key { get; set; }

        bool IsHidden { get; set; }

        long MaximumRuntimeInMilliseconds { get; set; }
    }
}