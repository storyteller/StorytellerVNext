using System;
using Baseline;

namespace Storyteller.Model
{
    public class MissingFixtureException : Exception
    {
        public MissingFixtureException(string key)
            : base("Fixture with key '{0}' does not exist".ToFormat(key))
        {
        }
    }
}