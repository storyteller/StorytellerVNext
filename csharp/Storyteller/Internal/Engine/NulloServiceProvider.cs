using System;

namespace Storyteller.Internal.Engine
{
    internal class NulloServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}