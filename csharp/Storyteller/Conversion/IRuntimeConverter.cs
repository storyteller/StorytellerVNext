using System;

namespace Storyteller.Conversion
{
    // SAMPLE: IRuntimeConverter
    public interface IRuntimeConverter
    {
        object Convert(string raw, IExecutionContext context);
        bool Matches(Type type);
    }
    // ENDSAMPLE
}