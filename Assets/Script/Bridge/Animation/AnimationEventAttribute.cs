using System;

namespace YBFramework.MyEditor
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class AnimationEventAttribute : Attribute
    {
        public readonly string EventName;

        public readonly int EventIndex;

        public AnimationEventAttribute(string eventName, int eventIndex)
        {
            EventName = eventName;
            EventIndex = eventIndex;
        }
    }
}