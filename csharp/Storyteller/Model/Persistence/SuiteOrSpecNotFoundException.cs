using System;
using Baseline;

namespace Storyteller.Model.Persistence
{
    public class SuiteOrSpecNotFoundException : Exception
    {
        public SuiteOrSpecNotFoundException(string suite, Suite top)
            : base("Unable to find a suite or specification with the path '{0}' in {1}".ToFormat(suite, top.Folder)) { }
    }
}