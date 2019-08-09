using Storyteller.Model;

namespace Storyteller.Internal.Engine
{
    public interface IGrammar
    {

        void CreatePlan(ExecutionPlan plan, Step step, FixtureLibrary library, bool inTable = false);

        GrammarModel Compile(Fixture fixture, CellHandling cells);

        string Key { get; set; }

        bool IsHidden { get; set; }

        long MaximumRuntimeInMilliseconds { get; set; }
    }
}