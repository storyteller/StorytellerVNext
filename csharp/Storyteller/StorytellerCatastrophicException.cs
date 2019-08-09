using System;

namespace Storyteller
{
    /// <summary>
    /// Means that the entire system under test is invalid and
    /// no more Specifications should be executed
    /// </summary>
#if NET46
    [Serializable]
#endif
    public class StorytellerCatastrophicException : StorytellerFailureException
    {
        public StorytellerCatastrophicException()
        {
        }

        public StorytellerCatastrophicException(string message) : base(message, Storyteller.Results.ErrorDisplay.text)
        {
        }

        public StorytellerCatastrophicException(string message, Exception innerException) : base(message, innerException, Storyteller.Results.ErrorDisplay.text)
        {
        }

#if NET46
        protected StorytellerCatastrophicException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}