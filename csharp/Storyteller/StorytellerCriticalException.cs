using System;

namespace Storyteller
{
    /// <summary>
    /// Means that the current spec fun is invalid and should be 
    /// stopped immediately
    /// </summary>
#if NET46
    [Serializable]
#endif
    public class StorytellerCriticalException : StorytellerFailureException
    {
        public StorytellerCriticalException()
        {
        }

        public StorytellerCriticalException(string message) : base(message, Storyteller.Results.ErrorDisplay.text)
        {
        }

        public StorytellerCriticalException(string message, Exception innerException) : base(message, innerException, Storyteller.Results.ErrorDisplay.text)
        {
        }

#if NET46
        protected StorytellerCriticalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}