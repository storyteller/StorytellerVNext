using System;

namespace Storyteller.Equivalence
{
    public interface IEquivalenceCheckerPolicy
    {
        bool Matches(Type type);
        Func<object, object, bool> CreateComparison(Type type, EquivalenceChecker checker);
    }
}