using System;

namespace YBFramework.MyEditor
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public sealed class AnimationEventSourceAttribute : Attribute
    {
        public readonly string EventSourceName;

        public AnimationEventSourceAttribute(string eventSourceName)
        {
            EventSourceName = eventSourceName;
        }
    }
}