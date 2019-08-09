namespace Storyteller.Results
{
    public interface IResultMessage
    {
        void Tabulate(Counts counts);

        // ReSharper disable once InconsistentNaming
        string id { get; set; }

        string type { get; }

        string spec { get; set; }

        void MarkPerformance(PerfRecord record);

        long duration { get; set; }
    }
}