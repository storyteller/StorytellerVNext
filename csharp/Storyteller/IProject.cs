using System.Collections.Generic;

namespace Storyteller
{
    public interface IProject
    {
        string Profile { get; set; }
        string Culture { get; set; }

        Dictionary<string, string> Properties { get; set; }

        string ProjectPath { get; set; }
        string SpecDirectory { get; set; }
    }
}