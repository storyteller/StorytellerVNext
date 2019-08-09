using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Baseline;

namespace Storyteller.Equivalence
{
    public class ArrayEquivalencyPolicy : IEquivalenceCheckerPolicy
    {
        public bool Matches(Type type)
        {
            return type.IsArray;
        }

        public Func<object, object, bool> CreateComparison(Type type, EquivalenceChecker checker)
        {
            var inner = checker.CheckerFor(type.GetElementType());
            return (expected, actual) => new CollectionComparer(inner).Matches(expected, actual);
        }
    }

    public class CollectionEquivalencyPolicy : IEquivalenceCheckerPolicy
    {
        public bool Matches(Type type)
        {
            return type.Closes(typeof (List<>));
        }

        public Func<object, object, bool> CreateComparison(Type type, EquivalenceChecker checker)
        {
            var elementType = type.GetTypeInfo().GetGenericArguments().First();
            var inner = checker.CheckerFor(elementType);

            return (expected, actual) => new CollectionComparer(inner).Matches(expected, actual);
        }
    }

    public class CollectionComparer
    {
        private readonly Func<object, object, bool> _inner;

        public CollectionComparer(Func<object, object, bool> inner)
        {
            _inner = inner;
        }

        public bool Matches(object one, object two)
        {
            var expected = new List<object>(one.As<IEnumerable>().OfType<object>());
            var actual = new List<object>(two.As<IEnumerable>().OfType<object>());

            foreach (object o in actual.ToArray())
            {
                if (expected.Contains(o))
                {
                    actual.Remove(o);
                    expected.Remove(o);
                }
            }

            foreach (object o in expected.ToArray())
            {
                if (actual.Contains(o))
                {
                    actual.Remove(o);
                    expected.Remove(o);
                }
            }

            return actual.Count == 0 && expected.Count == 0;
        }
    }
}