using System;
using System.Threading;
using Storyteller.Internal.Engine;
using Storyteller.Model;
using Storyteller.Results;

namespace Storyteller
{
    public interface IExecutionContext
    {
        
        /// <summary>
        /// Information about the current specification project including
        /// file paths
        /// </summary>
        IProject Project { get; }
        
        /// <summary>
        /// Delegates to the service locator in the
        /// IExecutionContext for this specification execution
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IServiceProvider Services { get; }


        /// <summary>
        /// A state bag of objects used to share state
        /// between grammars and fixtures within the
        /// execution of a specification
        /// </summary>
        State State { get; }

        
        /// <summary>
        /// The current "counts" of rights, wrongs,
        /// errors, and invalid date reported by the
        /// engine so far
        /// </summary>
        Counts Counts { get; }

        /// <summary>
        /// The currently executing Specification
        /// </summary>
        Specification Specification { get; }

        /// <summary>
        /// Can be used to add extra contextual information
        /// to the performance tracing
        /// </summary>
        Timings Timings { get; }

        /// <summary>
        /// Can be used to add custom html tracing to 
        /// the specification results
        /// </summary>
        IReporting Reporting { get; }
        
        
        
        /// <summary>
        /// Check this for whether or not the current execution
        /// has been cancelled
        /// </summary>
        CancellationToken Cancellation { get; }
        
        
        
        /// <summary>
        /// Yet another implementation of a polling wait until
        /// this condition is true, but this one will stop
        /// polling if this specification context has been
        /// cancelled
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="timeout"></param>
        /// <param name="millisecondPolling"></param>
        /// <returns></returns>
        bool Wait(Func<bool> condition, TimeSpan timeout, int millisecondPolling = 500);

        void LogResult<T>(T result, PerfRecord record) where T : IResultMessage;
        void LogException(string id, Exception ex, PerfRecord record, object position = null);
    }
}