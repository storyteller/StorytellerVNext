using System;

namespace Storyteller
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class HiddenAttribute : Attribute
    {
    }
}