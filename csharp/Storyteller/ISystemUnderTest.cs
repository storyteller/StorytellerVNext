using System.Threading.Tasks;
using Storyteller.Model;

namespace Storyteller
{
    public interface ISystemUnderTest
    {
        FixtureLibrary Library { get; }
        Task<IExecutionContext> CreateContext(Specification specification);

        // Does all the afters, reports the results, builds out SpecResults
        Task Complete(IExecutionContext context);
    }
}