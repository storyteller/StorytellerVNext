using Baseline;
using Storyteller.Conversion;
using Storyteller.Internal.Engine;

namespace Storyteller.Model
{
    public class MissingGrammar : GrammarModel, IGrammar, IModelWithCells
    {
        private readonly string _message;

        public MissingGrammar(string key) : base("missing")
        {
            _message = "Grammar '{0}' is not implemented".ToFormat(key);
            AddError(new GrammarError {error = _message});
        }

        public void CreatePlan(ExecutionPlan plan, StepValues values, FixtureLibrary library, bool inTable = false)
        {
            throw new System.NotImplementedException();
        }

        GrammarModel IGrammar.Compile(Fixture fixture, CellHandling cells)
        {
            return this;
        }

        public Cell[] cells => new Cell[0];

        public string Key { get; set; }
        public override string TitleOrFormat()
        {
            return "MISSING";
        }

        public long MaximumRuntimeInMilliseconds { get; set; }
    }
}