using System;
using Storyteller.Model.Persistence;

namespace Storyteller.Internal
{
    public interface ISpecificationRunner : IDisposable
    {
        Hierarchy Hierarchy { get; }

        Project Project { get; }
        
    }
}