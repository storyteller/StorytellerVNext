using System.Collections.Generic;

namespace Storyteller.Model
{
    public interface INodeHolder
    {
        IList<Node> Children { get; }
    }
}